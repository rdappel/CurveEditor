#pragma once

#include "pch.h"

using namespace KatanaEngine;

// Draws the curves being edited, the way XNA's Curve Editor did: a grid with numbered
// lines, each curve (dimmed past its first and last keys), its keys, the tangent arrows of
// the active keys and the selection box. Everything shown comes from host messages; the
// curves are evaluated with the engine's Curve, so they look exactly as a game will see them.
//
// The view maps curve units to pixels with an offset (the curve point at the bottom left)
// and a scale (curve units per pixel) on each axis. Mouse input is forwarded to the host as
// pixel positions, which does all the editing.
class CurveViewScreen : public Screen
{
public:

	// Which keys or tangents are drawn.
	enum class ShowMode { ALWAYS, NEVER, ACTIVE_ONLY };

	// A key's selection bits, as sent by the host.
	static constexpr int SELECT_TANGENT_IN = 1;
	static constexpr int SELECT_TANGENT_OUT = 2;
	static constexpr int SELECT_KEY = 4;

	CurveViewScreen() = default;
	~CurveViewScreen() override = default;

	// Applies a curves, curve, view, display, box or cursor message (see CurveEditorGame.h).
	// Returns false if the message isn't one of those; sets error if it is but is malformed.
	bool ApplyMessage(const std::string& message, std::string& error);

	void SetMessageSender(std::function<void(const std::string&)> sender) { m_sendMessage = std::move(sender); }

	void LoadContent(ResourceManager& resourceManager) override;
	void HandleInput(const InputState& input) override;
	void Draw(SpriteBatch& spriteBatch) override;

	static const Color BackgroundColor;

private:

	struct ViewCurve
	{
		Curve Keys;
		Color LineColor;
		bool IsVisible = true;
		bool IsEditable = false;			// selected in the host's curve list
		std::vector<int> Selection;			// selection bits for each key, in key order
	};

	bool ParseCurve(const std::string& arguments, std::string& error);

	Vector2 ToPixel(double position, double value) const;
	float GetKeySpacing(const ViewCurve& curve, int index, bool isOut) const;

	void DrawGrid(SpriteBatch& spriteBatch) const;
	void DrawCurve(SpriteBatch& spriteBatch, const ViewCurve& curve) const;
	void DrawSamples(SpriteBatch& spriteBatch, const Curve& curve, const Color& color, double from, double to, double step) const;
	void DrawKeySegments(SpriteBatch& spriteBatch, const Curve& curve, const Color& color, double from, double to, double step) const;
	void DrawTangents(SpriteBatch& spriteBatch, const ViewCurve& curve) const;
	void DrawArrow(SpriteBatch& spriteBatch, const Vector2& origin, float angle, const Color& color) const;
	void DrawKeys(SpriteBatch& spriteBatch) const;
	void DrawLine(SpriteBatch& spriteBatch, const Vector2& from, const Vector2& to, const Color& color) const;

	void SendMouseMessage(const char* action, const Vector2& position, const char* button) const;

	std::vector<ViewCurve> m_curves;

	double m_offsetX = -1.0, m_offsetY = -1.0;	// the curve point at the view's bottom left
	double m_scaleX = 0.01, m_scaleY = 0.01;	// curve units per pixel

	int m_smoothness = 4;						// pixels between curve samples
	ShowMode m_showKeys = ShowMode::ALWAYS;
	ShowMode m_showTangents = ShowMode::ACTIVE_ONLY;
	bool m_showInfinity = true;

	bool m_hasBox = false;
	float m_boxX = 0, m_boxY = 0, m_boxWidth = 0, m_boxHeight = 0;	// pixels

	std::shared_ptr<Font> m_font;
	Vector2 m_lastMousePosition;
	std::function<void(const std::string&)> m_sendMessage;
};
