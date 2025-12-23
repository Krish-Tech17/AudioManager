# AudioManager

## Overview

This system provides a **queue-based narration solution** for Unity projects, supporting both:

* 🎵 **Audio clips** (pre-recorded narration)
* 🗣 **Text-to-Speech (TTS)** (platform-dependent)

Both managers follow the **same design philosophy**:

* Singleton-based
* Queue-driven playback
* Ability to interrupt and override current narration
* Persistent across scenes

This makes them ideal for **training apps, VR simulations, guided walkthroughs, and safety scenarios**.

---

## Architecture

```
AudioManager  ---> Handles AudioClip narration
TTSManager    ---> Handles spoken text narration
```

Each manager:

* Plays one item at a time
* Automatically moves to the next queued item
* Ensures narration does not overlap

---

## 🎵 AudioManager

### Purpose

Handles **AudioClip-based narration**, ensuring:

* Clips play sequentially
* No overlapping audio
* Clean interruption when required

### Key Features

* Queue-based audio playback
* Immediate override support
* Fallback audio support
* Mute & volume control
* Scene-persistent singleton

### Setup

1. Add `AudioManager` to a GameObject
2. Assign an `AudioSource` (or let it auto-fetch)
3. (Optional) Assign a `fallbackClip`

```csharp
DontDestroyOnLoad(gameObject);
```

### Public API

#### Play audio in sequence

```csharp
AudioManager.Instance.PlayClipInQueue(audioClip);
```

#### Stop current audio and play immediately

```csharp
AudioManager.Instance.StopAndPlay(audioClip);
```

#### Stop all narration

```csharp
AudioManager.Instance.Stop();
```

#### Play fallback narration

```csharp
AudioManager.Instance.PlayFallback();
```

#### Volume control

```csharp
AudioManager.Instance.SetVolume(0.8f);
```

#### Mute / Unmute

```csharp
AudioManager.Instance.Mute(true);
```

### Internal Behavior

* Uses a `Queue<AudioClip>`
* Plays next clip only after the current one finishes
* Automatically advances narration

---

## 🗣 TTSManager (Text-to-Speech)

### Purpose

Handles **spoken narration generated from text**, following the **same queue and override behavior** as `AudioManager`.

### Supported Platforms

* ✅ Windows (via `WindowsTTS`)
* ⚠ Android / iOS / macOS (placeholders added)

### Key Features

* Queue-based TTS narration
* Immediate interruption support
* Platform-specific implementations
* Scene-persistent singleton

### Public API

#### Speak text in queue

```csharp
TTSManager.Instance.SpeakInQueue("Wear your safety helmet");
```

#### Stop current speech and speak immediately

```csharp
TTSManager.Instance.StopAndSpeak("Emergency detected");
```

#### Stop all TTS narration

```csharp
TTSManager.Instance.Stop();
```

### Internal Behavior

* Uses a `Queue<string>`
* Tracks speech completion via platform callbacks
* Automatically continues queued narration

---

## 🔄 Design Consistency

Both managers share:

* Same method naming pattern (`PlayClipInQueue`, `SpeakInQueue`)
* Same interruption logic (`StopAndPlay`, `StopAndSpeak`)
* Same sequential playback model

This makes them easy to use together or swap based on narration type.

---


---

## Use Cases

* VR / AR training simulations
* Safety & emergency guidance
* Interactive walkthroughs
* Accessibility narration


---

## Author

**Vijay Kumar**
Senior Unity Developer
