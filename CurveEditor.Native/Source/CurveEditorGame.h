#pragma once

#include "pch.h"

using namespace KatanaEngine;

class CurveViewScreen;

// The native side of CurveEditor, hosted inside the WinForms app's viewport. It only draws
// and reports the mouse; the host owns the curves and does all the editing.
//
// Host -> game messages:
//     curves <count>        how many curves there are (new ones start empty)
//     curve <index> <r>,<g>,<b> <visible 0|1> <editable 0|1> <preLoop> <postLoop> <keys>
//                           a curve's contents. Loops are 0-4 (Constant, Cycle, CycleOffset,
//                           Oscillate, Linear). keys is "-" or, for each key in position order,
//                           "position,value,tangentIn,tangentOut,step 0|1,selection;" where
//                           selection has bits 1 (in tangent), 2 (out tangent) and 4 (key)
//     view <offsetX> <offsetY> <scaleX> <scaleY>
//                           the curve point at the bottom left, and curve units per pixel
//     display <smoothness> <keys> <tangents> <infinity 0|1>
//                           pixels between curve samples; which keys and tangents are drawn:
//                           0 always, 1 never, 2 active only; whether to draw past the end keys
//     box <x> <y> <width> <height> | box none
//                           the selection box, in pixels
//     cursor default | arrow | precision | move | hand | resize
// Game -> host messages:
//     ready                 content loaded
//     mouse down|up <x> <y> left|right|middle, mouse move <x> <y>
//                           in pixels (moves only while a button is held)
//     wheel <notches> <x> <y>
//     error <text>          a message could not be applied
class CurveEditorGame : public Game
{
public:

	explicit CurveEditorGame(const GameConfig& config);
	~CurveEditorGame() override = default;

protected:

	void LoadContent(ResourceManager& resourceManager) override;
	void HandleInput(const InputState& input) override;
	void Update(const GameTime& gameTime) override;
	void Draw(SpriteBatch& spriteBatch) override;
	void OnHostMessage(const std::string& message) override;

private:

	ScreenManager& m_screenManager = GetScreenManager();
	std::shared_ptr<CurveViewScreen> m_viewScreen;
};
