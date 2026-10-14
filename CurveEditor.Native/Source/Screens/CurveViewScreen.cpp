#include "pch.h"
#include "CurveViewScreen.h"

#include <cstdio>
#include <cstring>
#include <limits>
#include <sstream>

namespace
{
	Color Rgb(int red, int green, int blue, int alpha = 255)
	{
		return Color(red / 255.0f, green / 255.0f, blue / 255.0f, alpha / 255.0f);
	}

	const Color GridLineColor = Rgb(200, 205, 211);
	const Color GridAxisColor = Rgb(100, 102, 106);
	const Color GridTextColor = Rgb(0, 0, 0);
	const Color KeyColor = Rgb(0, 0, 0);
	const Color SelectedColor = Rgb(255, 255, 0);			// selected keys and tangents
	const Color TangentColor = Rgb(165, 42, 42);			// unselected tangents (brown)
	const Color BoxFillColor = Rgb(0, 200, 128, 80);
	const Color BoxBorderColor = Rgb(0, 0, 0);

	constexpr double MinGridSpacing = 50.0;					// pixels between grid lines, at least
	constexpr float GridTextMargin = 2.0f;
	constexpr float TangentArrowLength = 50.0f;
	constexpr float KeySize = 4.0f;
	constexpr float InfinityDimming = 0.75f;				// how far the extrapolated curve fades to the background

	bool StartsWith(const std::string& text, const std::string& prefix)
	{
		return text.compare(0, prefix.size(), prefix) == 0;
	}

	// The spacing for grid lines about idealSpacing apart: 0.1, 0.2, 0.5, 1, 2, 4 or 5 times a power of ten.
	double ComputeGridSpacing(double idealSpacing)
	{
		static const double Multiples[] = { 0.1, 0.2, 0.5, 1.0, 2.0, 4.0, 5.0 };
		const double power = std::pow(10.0, std::trunc(std::log10(idealSpacing)));

		double best = power;
		double bestDistance = std::numeric_limits<double>::max();
		for (double multiple : Multiples)
		{
			const double distance = std::abs(power * multiple - idealSpacing);
			if (distance >= bestDistance) continue;

			bestDistance = distance;
			best = power * multiple;
		}
		return best;
	}

	std::string FormatGridNumber(double value)
	{
		char text[32];
		std::snprintf(text, sizeof(text), "%.9g", std::round(value * 1.0e9) / 1.0e9);
		return std::strcmp(text, "-0") == 0 ? "0" : text;
	}

	bool ParseLoopType(int value, CurveLoopType& loop)
	{
		if (value < 0 || value > static_cast<int>(CurveLoopType::LINEAR)) return false;
		loop = static_cast<CurveLoopType>(value);
		return true;
	}

	bool ParseShowMode(int value, CurveViewScreen::ShowMode& mode)
	{
		if (value < 0 || value > static_cast<int>(CurveViewScreen::ShowMode::ACTIVE_ONLY)) return false;
		mode = static_cast<CurveViewScreen::ShowMode>(value);
		return true;
	}

	bool ParseCursor(const std::string& name, MouseCursor& cursor)
	{
		static const std::pair<const char*, MouseCursor> Cursors[] = {
			{ "default", MouseCursor::DEFAULT }, { "arrow", MouseCursor::ARROW },
			{ "precision", MouseCursor::PRECISION }, { "move", MouseCursor::MOVE },
			{ "hand", MouseCursor::LINK }, { "resize", MouseCursor::RESIZE_N } };

		for (const auto& [cursorName, value] : Cursors)
		{
			if (name != cursorName) continue;
			cursor = value;
			return true;
		}
		return false;
	}

	std::istringstream MakeStream(const std::string& text)
	{
		std::istringstream stream(text);
		stream.imbue(std::locale::classic());
		return stream;
	}
}

const Color CurveViewScreen::BackgroundColor = Rgb(228, 233, 240);

void CurveViewScreen::LoadContent(ResourceManager& resourceManager)
{
	// Grid numbers; the editor only runs on Windows.
	char* windowsFolder = nullptr;
	size_t length = 0;
	const std::string fonts = _dupenv_s(&windowsFolder, &length, "WINDIR") == 0 && windowsFolder
		? std::string(windowsFolder) + "/Fonts/" : "C:/Windows/Fonts/";
	std::free(windowsFolder);

	m_font = resourceManager.Load<Font>(fonts + "segoeui.ttf@12");
	if (!m_font) m_font = resourceManager.Load<Font>(fonts + "arial.ttf@12");
}

// ---- Messages ----

bool CurveViewScreen::ApplyMessage(const std::string& message, std::string& error)
{
	const size_t space = message.find(' ');
	const std::string command = message.substr(0, space);
	const std::string arguments = space == std::string::npos ? std::string() : message.substr(space + 1);
	auto stream = MakeStream(arguments);

	if (command == "curves")
	{
		int count = -1;
		if (!(stream >> count) || count < 0) error = "bad curve count: " + arguments;
		else m_curves.resize(count);
		return true;
	}

	if (command == "curve")
	{
		ParseCurve(arguments, error);
		return true;
	}

	if (command == "view")
	{
		double offsetX, offsetY, scaleX, scaleY;
		if (!(stream >> offsetX >> offsetY >> scaleX >> scaleY) || scaleX <= 0.0 || scaleY <= 0.0) error = "bad view: " + arguments;
		else
		{
			m_offsetX = offsetX;
			m_offsetY = offsetY;
			m_scaleX = scaleX;
			m_scaleY = scaleY;
		}
		return true;
	}

	if (command == "display")
	{
		int smoothness, keys, tangents, infinity;
		ShowMode showKeys, showTangents;
		if (!(stream >> smoothness >> keys >> tangents >> infinity) || smoothness < 1
			|| !ParseShowMode(keys, showKeys) || !ParseShowMode(tangents, showTangents)) error = "bad display: " + arguments;
		else
		{
			m_smoothness = smoothness;
			m_showKeys = showKeys;
			m_showTangents = showTangents;
			m_showInfinity = infinity != 0;
		}
		return true;
	}

	if (command == "box")
	{
		if (arguments == "none") m_hasBox = false;
		else if (stream >> m_boxX >> m_boxY >> m_boxWidth >> m_boxHeight) m_hasBox = true;
		else error = "bad box: " + arguments;
		return true;
	}

	if (command == "cursor")
	{
		MouseCursor cursor;
		if (ParseCursor(arguments, cursor)) GetScreenManager()->GetGame().SetMouseCursor(cursor);
		else error = "unknown cursor: " + arguments;
		return true;
	}

	return false;
}

// curve <index> <r>,<g>,<b> <visible 0|1> <editable 0|1> <preLoop> <postLoop> <keys>
// where keys is "-" (none) or "position,value,tangentIn,tangentOut,step,selection;..." in position order.
bool CurveViewScreen::ParseCurve(const std::string& arguments, std::string& error)
{
	auto stream = MakeStream(arguments);
	int index, red, green, blue, visible, editable, preLoop, postLoop;
	char comma1, comma2;
	std::string keysText;

	ViewCurve curve;
	if (!(stream >> index >> red >> comma1 >> green >> comma2 >> blue >> visible >> editable >> preLoop >> postLoop >> keysText)
		|| index < 0 || index >= static_cast<int>(m_curves.size()) || comma1 != ',' || comma2 != ','
		|| !ParseLoopType(preLoop, curve.Keys.PreLoop) || !ParseLoopType(postLoop, curve.Keys.PostLoop))
	{
		error = "bad curve: " + arguments;
		return false;
	}

	curve.LineColor = Rgb(red, green, blue);
	curve.IsVisible = visible != 0;
	curve.IsEditable = editable != 0;

	if (keysText != "-")
	{
		std::replace(keysText.begin(), keysText.end(), ',', ' ');
		std::replace(keysText.begin(), keysText.end(), ';', ' ');
		auto keyStream = MakeStream(keysText);

		CurveKey key;
		int step, selection;
		while (keyStream >> key.Position >> key.Value >> key.TangentIn >> key.TangentOut >> step >> selection)
		{
			key.Continuity = step ? CurveContinuity::STEP : CurveContinuity::SMOOTH;
			curve.Keys.AddKey(key);		// keys arrive in order, so this appends
			curve.Selection.push_back(selection);
		}

		if (!keyStream.eof())
		{
			error = "bad curve keys: " + arguments;
			return false;
		}
	}

	m_curves[index] = std::move(curve);
	return true;
}

// ---- Input ----

void CurveViewScreen::HandleInput(const InputState& input)
{
	if (!m_sendMessage) return;

	const Vector2 position = input.GetMousePosition();
	const bool isDragging = input.IsMouseButtonDown(MouseButton::LEFT) || input.IsMouseButtonDown(MouseButton::RIGHT);

	if (input.IsMouseButtonPressed(MouseButton::LEFT)) SendMouseMessage("down", position, "left");
	if (input.IsMouseButtonPressed(MouseButton::RIGHT)) SendMouseMessage("down", position, "right");
	if (isDragging && position != m_lastMousePosition) SendMouseMessage("move", position, nullptr);
	if (input.IsMouseButtonReleased(MouseButton::LEFT)) SendMouseMessage("up", position, "left");
	if (input.IsMouseButtonReleased(MouseButton::RIGHT)) SendMouseMessage("up", position, "right");

	const int wheel = input.GetMouseWheelDelta();
	if (wheel != 0)
	{
		char message[64];
		std::snprintf(message, sizeof(message), "wheel %d %.1f %.1f", wheel, position.X, position.Y);
		m_sendMessage(message);
	}

	m_lastMousePosition = position;
}

void CurveViewScreen::SendMouseMessage(const char* action, const Vector2& position, const char* button) const
{
	char message[96];
	std::snprintf(message, sizeof(message), "mouse %s %.1f %.1f%s%s", action, position.X, position.Y,
		button ? " " : "", button ? button : "");
	m_sendMessage(message);
}

// ---- Drawing ----

Vector2 CurveViewScreen::ToPixel(double position, double value) const
{
	const double height = GetScreenManager()->GetGame().GetScreenHeight();
	return Vector2(static_cast<float>((position - m_offsetX) / m_scaleX), static_cast<float>(height - (value - m_offsetY) / m_scaleY));
}

// The distance to the neighbouring key that a tangent's slope is measured over (1 at the
// ends, or the last segment's when the curve oscillates there), as in XNA's Curve Editor.
float CurveViewScreen::GetKeySpacing(const ViewCurve& curve, int index, bool isOut) const
{
	const Curve& keys = curve.Keys;
	const int count = keys.GetKeyCount();

	if (!isOut)
	{
		if (index > 0) return keys.GetKey(index).Position - keys.GetKey(index - 1).Position;
		if (keys.PreLoop == CurveLoopType::OSCILLATE && count > 1) return keys.GetKey(1).Position - keys.GetKey(0).Position;
		return 1.0f;
	}

	if (index < count - 1) return keys.GetKey(index + 1).Position - keys.GetKey(index).Position;
	if (keys.PostLoop == CurveLoopType::OSCILLATE && count > 1) return keys.GetKey(index).Position - keys.GetKey(index - 1).Position;
	return 1.0f;
}

void CurveViewScreen::Draw(SpriteBatch& spriteBatch)
{
	spriteBatch.Begin();

	DrawGrid(spriteBatch);
	for (const ViewCurve& curve : m_curves) if (curve.IsVisible) DrawCurve(spriteBatch, curve);
	if (m_showTangents != ShowMode::NEVER) for (const ViewCurve& curve : m_curves) if (curve.IsVisible) DrawTangents(spriteBatch, curve);
	if (m_showKeys != ShowMode::NEVER) DrawKeys(spriteBatch);

	if (m_hasBox)
	{
		spriteBatch.DrawPrimitive(Rectangle(m_boxX, m_boxY, m_boxWidth, m_boxHeight), BoxFillColor, Color::TRANSPARENT);
		spriteBatch.DrawPrimitive(Rectangle(std::floor(m_boxX) + 0.5f, std::floor(m_boxY) + 0.5f,
			std::floor(m_boxWidth), std::floor(m_boxHeight)), Color::TRANSPARENT, BoxBorderColor);
	}

	spriteBatch.End();
}

void CurveViewScreen::DrawLine(SpriteBatch& spriteBatch, const Vector2& from, const Vector2& to, const Color& color) const
{
	spriteBatch.DrawPrimitive(Line(from, to), color);
}

void CurveViewScreen::DrawGrid(SpriteBatch& spriteBatch) const
{
	const Game& game = GetScreenManager()->GetGame();
	const float width = static_cast<float>(game.GetScreenWidth());
	const float height = static_cast<float>(game.GetScreenHeight());
	if (width < MinGridSpacing || height < MinGridSpacing) return;

	const double spacingX = ComputeGridSpacing(MinGridSpacing * m_scaleX);
	const double spacingY = ComputeGridSpacing(MinGridSpacing * m_scaleY);
	const double startX = m_offsetX - std::fmod(m_offsetX, spacingX);
	const double startY = m_offsetY - std::fmod(m_offsetY, spacingY);
	const int countX = static_cast<int>(std::ceil(width * m_scaleX / spacingX)) + 1;
	const int countY = static_cast<int>(std::ceil(height * m_scaleY / spacingY)) + 1;
	constexpr double AxisThreshold = 1.0e-8;

	// Lines, with the axes darker.
	for (int i = 0; i < countX; ++i)
	{
		const double position = startX + i * spacingX;
		const float x = std::floor(static_cast<float>((position - m_offsetX) / m_scaleX)) + 0.5f;
		DrawLine(spriteBatch, Vector2(x, 0.0f), Vector2(x, height), std::abs(position) < AxisThreshold * spacingX ? GridAxisColor : GridLineColor);
	}
	for (int i = 0; i < countY; ++i)
	{
		const double value = startY + i * spacingY;
		const float y = height - std::floor(static_cast<float>((value - m_offsetY) / m_scaleY)) - 0.5f;
		DrawLine(spriteBatch, Vector2(0.0f, y), Vector2(width, y), std::abs(value) < AxisThreshold * spacingY ? GridAxisColor : GridLineColor);
	}

	if (!m_font) return;

	// Numbers along the bottom, skipping any that would overlap the previous one...
	const float textHeight = static_cast<float>(m_font->GetLineHeight());
	float lastTextEnd = -100.0f;
	for (int i = 0; i < countX; ++i)
	{
		const double position = startX + i * spacingX;
		const std::string text = FormatGridNumber(position);
		const float x = std::floor(static_cast<float>((position - m_offsetX) / m_scaleX));
		const float halfWidth = m_font->MeasureString(text) * 0.5f;
		if (x - halfWidth <= lastTextEnd) continue;

		lastTextEnd = x + halfWidth;
		spriteBatch.DrawString(m_font, text, Vector2(x - halfWidth, height - textHeight - GridTextMargin), GridTextColor);
	}

	// ...and up the left, clear of the bottom row.
	const float halfHeight = textHeight * 0.5f;
	const float bottomLimit = height - 2.0f * textHeight - GridTextMargin;
	float lastTextTop = height + 100.0f;
	for (int i = 0; i < countY; ++i)
	{
		const double value = startY + i * spacingY;
		const float y = height - std::floor(static_cast<float>((value - m_offsetY) / m_scaleY)) - halfHeight;
		if (y >= lastTextTop || y >= bottomLimit) continue;

		lastTextTop = y - halfHeight;
		spriteBatch.DrawString(m_font, FormatGridNumber(value), Vector2(GridTextMargin, y), GridTextColor);
	}
}

void CurveViewScreen::DrawCurve(SpriteBatch& spriteBatch, const ViewCurve& curve) const
{
	const Curve& keys = curve.Keys;
	const int count = keys.GetKeyCount();
	if (count == 0) return;

	const Color& color = curve.LineColor;
	const Color infinityColor(
		color.Red + (BackgroundColor.Red - color.Red) * InfinityDimming,
		color.Green + (BackgroundColor.Green - color.Green) * InfinityDimming,
		color.Blue + (BackgroundColor.Blue - color.Blue) * InfinityDimming);

	const double viewStart = m_offsetX;
	const double viewEnd = m_offsetX + GetScreenManager()->GetGame().GetScreenWidth() * m_scaleX;
	const double step = m_scaleX * m_smoothness;
	const double firstPosition = keys.GetKey(0).Position;
	const double lastPosition = keys.GetKey(count - 1).Position;

	// Before the first key and after the last: straight lines where the curve is straight.
	auto drawInfinity = [&](double from, double to, CurveLoopType loop)
	{
		if (!m_showInfinity || from >= to) return;
		if (count > 1 && loop != CurveLoopType::CONSTANT && loop != CurveLoopType::LINEAR)
		{
			DrawSamples(spriteBatch, keys, infinityColor, from, to, step);
			return;
		}

		const float fromF = static_cast<float>(from), toF = static_cast<float>(to);
		DrawLine(spriteBatch, ToPixel(from, keys.Evaluate(fromF)), ToPixel(to, keys.Evaluate(toF)), infinityColor);
	};

	drawInfinity(viewStart, std::min(viewEnd, firstPosition), keys.PreLoop);

	const double from = std::max(viewStart, firstPosition);
	const double to = std::min(viewEnd, lastPosition);
	if (from < to) DrawKeySegments(spriteBatch, keys, color, from, to, step);

	drawInfinity(std::min(viewEnd, lastPosition), viewEnd, keys.PostLoop);
}

// Samples the curve every step from from to past to.
void CurveViewScreen::DrawSamples(SpriteBatch& spriteBatch, const Curve& curve, const Color& color, double from, double to, double step) const
{
	Vector2 previous = ToPixel(from, curve.Evaluate(static_cast<float>(from)));
	for (double position = from; position < to; )
	{
		position += step;
		const Vector2 next = ToPixel(position, curve.Evaluate(static_cast<float>(position)));
		DrawLine(spriteBatch, previous, next, color);
		previous = next;
	}
}

// Draws the curve between its keys: sampled where smooth (hitting each key exactly), and
// as a flat line then a jump where a key steps.
void CurveViewScreen::DrawKeySegments(SpriteBatch& spriteBatch, const Curve& curve, const Color& color, double from, double to, double step) const
{
	const int count = curve.GetKeyCount();

	// The segment holding from: [key, next key].
	int nextIndex = 1;
	while (nextIndex < count - 1 && curve.GetKey(nextIndex).Position <= from) ++nextIndex;

	Vector2 previous = ToPixel(from, curve.Evaluate(static_cast<float>(from)));
	for (double position = from; position < to && nextIndex < count; ++nextIndex)
	{
		const CurveKey& key = curve.GetKey(nextIndex - 1);
		const CurveKey& nextKey = curve.GetKey(nextIndex);
		const double segmentEnd = std::min(to, static_cast<double>(nextKey.Position));

		if (key.Continuity == CurveContinuity::STEP)
		{
			const Vector2 corner = ToPixel(nextKey.Position, key.Value);
			const Vector2 next = ToPixel(nextKey.Position, nextKey.Value);
			DrawLine(spriteBatch, previous, corner, color);
			DrawLine(spriteBatch, corner, next, color);
			previous = next;
			position = segmentEnd;
			continue;
		}

		while (position < segmentEnd)
		{
			position = std::min(position + step, segmentEnd);
			const Vector2 next = ToPixel(position, curve.Evaluate(static_cast<float>(position)));
			DrawLine(spriteBatch, previous, next, color);
			previous = next;
		}
	}
}

void CurveViewScreen::DrawTangents(SpriteBatch& spriteBatch, const ViewCurve& curve) const
{
	const Curve& keys = curve.Keys;
	const double screenSlope = m_scaleX / m_scaleY;	// curve slope to screen slope

	for (int i = 0; i < keys.GetKeyCount(); ++i)
	{
		const CurveKey& key = keys.GetKey(i);
		const int selection = curve.Selection[i];
		const bool isActive = selection != 0 && curve.IsEditable;
		if ((!isActive && m_showTangents != ShowMode::ALWAYS) || key.Continuity != CurveContinuity::SMOOTH) continue;

		const Vector2 origin = ToPixel(key.Position, key.Value);
		const float inSlope = static_cast<float>(key.TangentIn / GetKeySpacing(curve, i, false) * screenSlope);
		const float outSlope = static_cast<float>(key.TangentOut / GetKeySpacing(curve, i, true) * screenSlope);

		DrawArrow(spriteBatch, origin, Math::PI + std::atan(inSlope), (selection & SELECT_TANGENT_IN) ? SelectedColor : TangentColor);
		DrawArrow(spriteBatch, origin, std::atan(outSlope), (selection & SELECT_TANGENT_OUT) ? SelectedColor : TangentColor);
	}
}

// A line TangentArrowLength long from origin at angle (counterclockwise, screen up), with a small head.
void CurveViewScreen::DrawArrow(SpriteBatch& spriteBatch, const Vector2& origin, float angle, const Color& color) const
{
	const float sine = std::sin(angle), cosine = std::cos(angle);
	const Vector2 tip = origin + Vector2(cosine, -sine) * TangentArrowLength;
	DrawLine(spriteBatch, origin, tip, color);

	// The head: a triangle 4 px long and 8 px wide at the tip, filled with lines.
	auto toScreen = [&](float along, float across) { return tip + Vector2(cosine * along - sine * across, -(sine * along + cosine * across)); };
	const Vector2 point = toScreen(4.0f, 0.0f);
	for (float across = -4.0f; across <= 4.0f; across += 0.5f) DrawLine(spriteBatch, toScreen(0.0f, across), point, color);
}

void CurveViewScreen::DrawKeys(SpriteBatch& spriteBatch) const
{
	auto drawKey = [&](const CurveKey& key, const Color& color)
	{
		const Vector2 position = ToPixel(key.Position, key.Value);
		spriteBatch.DrawPrimitive(Rectangle(std::floor(position.X) - KeySize / 2, std::floor(position.Y) - KeySize / 2, KeySize, KeySize), color, Color::TRANSPARENT);
	};

	// Unselected keys (all keys of curves not being edited)...
	if (m_showKeys == ShowMode::ALWAYS)
	{
		for (const ViewCurve& curve : m_curves)
		{
			if (!curve.IsVisible) continue;
			for (int i = 0; i < curve.Keys.GetKeyCount(); ++i)
				if (!(curve.Selection[i] & SELECT_KEY) || !curve.IsEditable) drawKey(curve.Keys.GetKey(i), KeyColor);
		}
	}

	// ...then selected keys on top.
	for (const ViewCurve& curve : m_curves)
	{
		if (!curve.IsVisible || !curve.IsEditable) continue;
		for (int i = 0; i < curve.Keys.GetKeyCount(); ++i)
			if (curve.Selection[i] & SELECT_KEY) drawKey(curve.Keys.GetKey(i), SelectedColor);
	}
}
