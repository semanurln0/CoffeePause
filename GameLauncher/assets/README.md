# Assets Folder

This folder contains game assets including images and other resources.

## Expected Files

### Background Images
- `background.png` or `background.jpg` - Main menu background image

### Card Images (for Spider Solitaire)
Place card images in the `cards/` subdirectory with the following naming convention:
- `A♠.png`, `2♠.png`, `3♠.png`, ..., `K♠.png` (Spades)
- `A♥.png`, `2♥.png`, `3♥.png`, ..., `K♥.png` (Hearts)
- `A♦.png`, `2♦.png`, `3♦.png`, ..., `K♦.png` (Diamonds)
- `A♣.png`, `2♣.png`, `3♣.png`, ..., `K♣.png` (Clubs)

Or use `.jpg` extension instead of `.png`.

## Notes

- If asset files are not found, the game will use fallback graphics
- The main menu will use a gradient background if no background image is provided
- Spider Solitaire will render simple text-based cards if card images are not available

## How Assets are Loaded

The `AssetManager` class handles loading all assets. Images are cached for performance.
Images should be placed in this `assets` folder which is expected to be in the same directory as the executable.
