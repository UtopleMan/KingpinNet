---
name: pa-ui-coding
description: UI coding rules. Use before writing, reviewing or editing UI code: controls, their rendering, colours, fonts, glyphs or other graphical aspects.
---
# UI coding rules

The UI draws on three shared homes: custom controls, `Palette` and `DesignAssets`. Code you touch is done when every rendering change lives in a control subclass, and every colour and design asset it uses resolves to `Palette` or `DesignAssets`.

## Custom controls

To change how a control renders, subclass it in a new control and put the rendering change there. Use the new control wherever that look is wanted: the same look is almost always needed elsewhere in the UI, and one subclass keeps it in one place. Before writing one, look for an existing subclass that already does it.

## Palette

The app's colours live in one global `Palette` class. Every colour in the UI is a `Palette` member, on controls, styles and drawing code alike. When a colour is missing, add it to `Palette` first, then use it from there.

A colour literal on a control (hex, RGB, a framework named colour) is the one thing this rule exists to stop; replace any you meet in code you touch with its `Palette` member.

## DesignAssets

Line types, glyphs and fonts live in one global `DesignAssets` class. Every line type, glyph and font in the UI comes from `DesignAssets`. When one is missing, add it to `DesignAssets` first, then use it from there.

If the project has no `Palette` or `DesignAssets` yet, create it in the first change that needs a colour or asset.
