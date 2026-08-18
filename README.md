# 🎰 Unity Slot Game Assignment

A 2D slot machine game developed in Unity as part of a game development assignment.

The project focuses on implementing core slot-machine gameplay, randomized reel outcomes, coin management, UI feedback, smooth reel animations, visual polish, and a browser-playable WebGL build.

---

## 🎮 Play Online

### ▶️ [Play the WebGL Build](https://asheedeliyangod.github.io/Unity-Slot-Game-Assignment/)

The game can be played directly in a modern WebGL-compatible browser without installing Unity.

---

## ✨ Features

- 🎰 3-reel slot machine
- 🎲 Randomized reel outcomes
- 🪙 Coin-based gameplay
- 💰 Coin balance management
- 🎯 Winning combination detection
- 🏆 Win and result feedback
- 🎞️ Smooth reel spinning animations
- 🎨 Pixel-art-inspired casino visuals
- ✨ Animated decorative neon elements
- 🖥️ Unity WebGL build
- 🌐 Browser-based gameplay

---

## 🎯 Gameplay

The player starts with a configurable number of coins and can use the Spin button to play the slot machine.

Each spin generates a randomized combination of symbols across the three reels.

After the reels finish spinning, the resulting combination is evaluated against the configured winning conditions.

Winning combinations provide a coin reward, while each spin consumes the configured spin cost.

The game provides visual feedback for the result and updates the player's coin balance accordingly.

---

## 🕹️ Controls

### Desktop

- **Spin Button** — Start a slot-machine spin
- **Mouse** — Interact with the game UI

### Web Browser

The game is designed to run directly in a modern WebGL-compatible browser.

---

## 🛠️ Built With

- **Unity 6**
- **C#**
- **Universal Render Pipeline (URP)**
- **Unity WebGL**
- **Git**
- **GitHub**

---

## ⚙️ Implementation

The project was developed with a focus on clean gameplay logic and separation of responsibilities.

Key implementation areas include:

- Slot-machine game flow
- Reel spinning and animation handling
- Randomized symbol selection
- Winning-condition evaluation
- Coin management
- Spin-cost handling
- Reward handling
- Result UI feedback
- Animated UI and decorative elements
- WebGL deployment

The gameplay systems are implemented using C# scripts and Unity UI components.

---

## 💰 Coin System

The game includes a dedicated coin-management system.

The coin system supports:

- Configurable starting coin balance
- Adding coins
- Removing coins
- Checking whether the player has enough coins
- Updating the coin UI
- Rewarding the player after successful outcomes

The system prevents a spin from consuming coins when the player does not have enough balance.

---

## 🎰 Slot Machine Logic

The slot machine uses randomized outcomes to determine the symbols displayed on the reels.

After the reels finish spinning, the resulting symbols are evaluated against the configured winning conditions.

The result is then presented through the game's UI, providing clear feedback to the player.

---

## 🎨 Visual Design

The game uses a pixel-art-inspired casino aesthetic.

Animated decorative neon elements were added around the slot machine to make the scene feel more lively and visually engaging without affecting the core gameplay mechanics.

The decorative elements are purely visual and do not affect the underlying slot-machine logic.

---

## 📂 Project Structure

```text
Unity-Slot-Game-Assignment/
│
├── Assets/
│   ├── Animations/
│   ├── Prefabs/
│   ├── Scripts/
│   ├── UI/
│   ├── Sounds/
│   └── ...
│
├── Build/
│   ├── Unity.data
│   ├── Unity.framework.js
│   ├── Unity.loader.js
│   └── Unity.wasm
│
├── Packages/
│
├── ProjectSettings/
│
├── TemplateData/
│
├── index.html
│
├── .gitignore
│
└── README.md
