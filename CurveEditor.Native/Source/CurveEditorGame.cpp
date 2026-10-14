#include "pch.h"
#include "CurveEditorGame.h"
#include "Screens/CurveViewScreen.h"


CurveEditorGame::CurveEditorGame(const GameConfig& config)
	: Game(config)
{
	SetTitle("Curve Editor");
}

void CurveEditorGame::LoadContent(ResourceManager& resourceManager)
{
	m_viewScreen = std::make_shared<CurveViewScreen>();
	m_viewScreen->SetMessageSender([this](const std::string& message) { SendHostMessage(message); });
	m_screenManager.AddScreen(m_viewScreen);

	SendHostMessage("ready");
}

void CurveEditorGame::HandleInput(const InputState& input)
{
	m_screenManager.HandleInput(input);
}

void CurveEditorGame::Update(const GameTime& gameTime)
{
	m_screenManager.Update(gameTime);
}

void CurveEditorGame::Draw(SpriteBatch& spriteBatch)
{
	Clear(CurveViewScreen::BackgroundColor);

	m_screenManager.Draw(spriteBatch);
}

void CurveEditorGame::OnHostMessage(const std::string& message)
{
	std::string error;
	if (!m_viewScreen->ApplyMessage(message, error)) error = "unknown message: " + message;
	if (!error.empty()) SendHostMessage("error " + error);
}
