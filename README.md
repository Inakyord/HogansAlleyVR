# HogansAlleyVR

A cross-platform reflex shooter inspired by the classic 'Hogan's Alley', built with Unity. Features mouse aiming for PC and gyroscope motion controls for iOS.

## 📥 Downloads

### 📱 iOS Build (Xcode Project)
[Download iOS Build Files](https://drive.google.com/file/d/1QAR-swdxhopsWHdVh0mvgiI7gYH5bhXe/view?usp=sharing)
* **Note:** This is a zipped Xcode project. To run it on an iPhone, you must unzip the folder on a Mac, open `Unity-iPhone.xcodeproj`, and deploy it to your device using Xcode.

### 💻 Windows Build (Playable EXE)
[Download Windows Executable](https://drive.google.com/file/d/1Mky6y0aphF_z0MypVD1uuhco34oGfMps/view?usp=sharing)
* **Note:** Extract the ZIP file and run `Hogans Alley VR.exe` to play immediately on PC.

---

## 📄 Documentation
For a detailed technical breakdown, including game mechanics, script explanations, and asset descriptions, please refer to the **[Documentation PDF](./EVI__Hogan_s_Alley_VR.pdf)** included in this repository.

---

## 🛠️ Running the Project in Unity

If you are downloading the source code to run inside the Unity Editor:

1. Open the project in Unity Hub.
2. Navigate to the `Assets/Scenes` folder.
3. **Double-click `MenuScene` to open it.**
4. Press the **Play** button.
   * *Important:* You must start from the `MenuScene` for the game flow (Main Menu -> Game -> Game Over) to work correctly.

---

## 🎮 How to Play

Your goal is simple: test your reflexes and aim! Enemies and innocents will pop up in the alley windows. You must identify the threats and take them down before the time runs out.

### Controls

The game automatically detects your platform and adjusts the control scheme:

| Platform | Aiming | Shooting |
| :--- | :--- | :--- |
| **PC / Mac** | Move the **Mouse** to look around. | Click **Left Mouse Button** to fire. |
| **iOS / Mobile** | **Physically rotate your phone** (Gyroscope) to aim like a "Magic Window". | **Tap anywhere** on the screen to fire. |

### Rules & Scoring

* **⏱️ Time Limit:** You have **40 seconds** to get the highest score possible.
* **✅ +1 Point:** Shoot an **Enemy** (Look for the guns!).
* **❌ -1 Point:** Shoot an **Innocent** (Civilians, Police, etc.).
* **Game Loop:** Targets appear in groups of 3. Once you shoot (or wait too long), the group disappears and a new one spawns instantly.
