<h1 align="center">XAML3D</h1>

<p align="center">
  <b>A drop-in UWP XAML control that renders 3D models with Direct3D 11.</b><br/>
  Declare a model in XAML, then rotate, scale, and move it from C#. No game engine needed.
</p>

<p align="center">
  <img alt="Platform" src="https://img.shields.io/badge/platform-UWP-blue" />
  <img alt="Language" src="https://img.shields.io/badge/C%2B%2B%2FCX-Direct3D%2011-purple" />
  <img alt="Min Windows" src="https://img.shields.io/badge/Windows%2010-10586%2B-0078D6" />
  <img alt="Toolset" src="https://img.shields.io/badge/Visual%20Studio-2017%20(v141)-5C2D91" />
</p>

---

## Overview

**XAML3D** puts a native Direct3D 11 renderer inside an ordinary XAML element. The `DXModel` control derives from `SwapChainPanel`, so it works like any other XAML control. You can size it, place it in layouts, apply render transforms, and handle touch and mouse manipulation events. It also draws a real 3D mesh.

```xml
<Page xmlns:dx="using:DirectXModel">
    <dx:DXModel Width="300" Height="300"
                ModelName="Teapot.cmo"
                ModelLoaded="OnModelLoaded" />
</Page>
```

## Features

| Feature | Description |
|---|---|
| **XAML-first** | Use `<dx:DXModel ModelName="..."/>` in markup. It is a WinRT component, so any UWP language can use it. |
| **`.cmo` model loading** | Loads Visual Studio compiled meshes, including materials, DGSL/Phong shaders, and `.dds` textures. |
| **Transform API** | `Transform.Translation`, `Transform.Rotation` (in degrees), and `Transform.Scale` as `float3` / `Vector3`. |
| **Camera API** | `LookAt`, `SetPosition`, `Walk`, `Strafe`, `Pitch`, `RotateY`, `SetLens`, plus read-only frustum properties. |
| **Transparent background** | Clears to transparent with premultiplied alpha, so the content behind the control shows through. |
| **VSync render loop** | Renders on a high-priority background thread and waits for each vertical blank. |
| **Robust device handling** | Handles resize, DPI and zoom changes, suspend trimming, and device-lost recovery. |
| **Skinned animation (internal)** | Includes a bone-based skinning pipeline (`SkinnedMeshRenderer`). |

## Repository layout

```
XAML3D/
├── XAML3D.sln                    # Visual Studio solution
├── DirectXModel/                 # C++/CX Windows Runtime Component (the control)
│   ├── DXModel.h/.cpp            # Public XAML control: load + render loop
│   ├── Camera.h/.cpp             # Public WinRT camera
│   ├── Transform.h/.cpp          # Public WinRT world transform
│   ├── Common/
│   │   ├── DirectXPanelBase.*    # SwapChainPanel + D3D device/swap-chain lifecycle
│   │   ├── DirectXHelper.h       # ThrowIfFailed, ReadDataAsync helpers
│   │   └── StepTimer.h           # Frame timer
│   ├── Animation/
│   │   ├── Animation.h           # SkinnedMeshRenderer (bone animation)
│   │   └── SkinningVertexShader.hlsl
│   ├── DirectXTex/DDSTextureLoader.*  # DDS texture loading
│   └── VSD3DStarter.h            # .cmo mesh parser, Graphics constant buffers
└── TestApp/                      # C# UWP sample app
    ├── BlankPage1.xaml           # Landing page -> navigates to MainPage
    ├── MainPage.xaml(.cs)        # Three DXModel controls with drag-to-rotate
    └── Teapot.cmo, Sphere.cmo, *.cso, *.dds  # Sample assets
```

See [ARCHITECTURE.md](ARCHITECTURE.md) for a detailed design description.

## Getting started

### Prerequisites

- Windows 10
- Visual Studio 2017 or later with:
  - **Universal Windows Platform development**
  - **Desktop development with C++** and **C++ Universal Windows Platform tools** (v141)
- Windows 10 SDK **10.0.14393.0** (minimum target 10.0.10586.0)

### Build and run

1. Open `XAML3D.sln` in Visual Studio.
2. Select a platform such as **x86** or **x64**. *Any CPU* doesn't work because the control is native code.
3. Set **TestApp** as the startup project and press **F5**.
4. Select the button on the landing page, then drag a model to rotate it.

## Using the control in your app

1. **Reference the component.** Add a project reference to `DirectXModel.vcxproj` from your UWP app.
2. **Prepare your model** (`.cmo`):
   1. Create a new **C++ DirectX 11 App** project in Visual Studio.
   2. Add your `.fbx`, `.obj`, or `.dae` model to that project. The Mesh Content Pipeline converts it to `.cmo`.
   3. Build the project. In the output folder, find the `.cmo`, compiled shaders (`.cso`), and textures (`.dds`).
   4. Copy those files to the root of your app and set **Build Action** to **Content**.
3. **Declare the control:**

```xml
<dx:DXModel x:Name="Model"
            Width="300" Height="300"
            ModelName="Teapot.cmo"
            ManipulationMode="All"
            ManipulationDelta="OnManipulationDelta"
            ModelLoaded="OnModelLoaded" />
```

4. **Drive it from code:**

```csharp
using System.Numerics;

// Spin, scale and move the model
Model.Transform.Rotation    = new Vector3(0, 45, 0);   // degrees
Model.Transform.Scale       = new Vector3(1.5f);
Model.Transform.Translation = new Vector3(0, -0.5f, 0);

// Move the camera
Model.Camera.LookAt(new Vector3(0, 1, -5), Vector3.Zero, Vector3.UnitY);
Model.Camera.Walk(0.5f);
```

> **Note:** `ModelLoaded` may be raised off the UI thread. Use `Dispatcher.RunAsync` before you update other UI elements.

## API reference

### `DirectXModel.DXModel` (derives from `SwapChainPanel`)

| Member | Kind | Description |
|---|---|---|
| `ModelName` | `string` property | Package-relative `.cmo` path. Setting it loads the model and starts rendering. |
| `Transform` | `Transform` property | World transform of the model. |
| `Camera` | `Camera` property | View and projection camera. |
| `Clear(Color)` | method | Clears the render target to a color. |
| `ModelLoaded` | `EventHandler<bool>` | Raised when loading completes. |

### `DirectXModel.Transform`

`Translation`, `Rotation` (Euler angles in degrees), `Scale`, all of type `float3` / `System.Numerics.Vector3`.

### `DirectXModel.Camera`

`SetPosition(x,y,z)`, `LookAt(pos,target,up)`, `SetLens(fovY,aspect,orientation,zn,zf)`, `Walk(d)`, `Strafe(d)`, `Pitch(a)`, `RotateY(a)`. Read-only properties: `NearZ`, `FarZ`, `Aspect`, `FovY`, `FovX`, `NearWindowWidth/Height`, `FarWindowWidth/Height`.

## Roadmap

- [ ] Change materials at runtime
- [ ] Change textures at runtime
- [ ] Control lighting (direction, color, point lights)
- [ ] Expose skinned animation playback through the public API

## Credits

- Swap chain panel base and helpers are adapted from Microsoft's XAML SwapChainPanel DirectX interop sample.
- `.cmo` loading and skinning come from the Visual Studio 3D Starter Kit (`VSD3DStarter`).
- `DDSTextureLoader` comes from DirectXTex / DirectX Tool Kit.
