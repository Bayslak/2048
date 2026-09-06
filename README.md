# 2048

A clean implementation of the classic **2048** sliding-tile puzzle, built with **Godot 4.6 (.NET / C#)**.

Merge matching tiles by sliding the board in four directions. Every merge doubles the tile's value and adds to your score. Reach the **2048** tile to win or fill the board with no moves left and it's game over.

---
## Gameplay

- Use the **arrow keys** to slide all tiles in a direction.
- When two tiles of the same value collide, they merge into one tile of double the value, and your score increases by that new value.
- A new tile (2 or 4) spawns after every move that actually changes the board.
- Reach **2048** to win. If the board fills up with no adjacent matching tiles and no empty cells, the game is over.
- Your **best score** is saved automatically and persists between sessions.