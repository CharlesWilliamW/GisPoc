# Highly Extensible GIS Architecture Proof-of-Concept (PoC)
### WinForms .NET 8 | GMap.NET | Spatial Geometry | Service-Oriented Architecture

A high-performance, modular Desktop GIS application built with **WinForms (.NET 8)** and **GMap.NET**. This repository demonstrates how to bridge complex spatial mathematics (geodesic math) with highly decoupled software architecture principles, eliminating common WinForms anti-patterns such as bloated code-behind (`Form1.cs`) and UI/Input event state conflicts.

---

## 🚀 Key Features & Highlights

- **Decoupled Architecture**: Transitioned from standard monolithic WinForms event handlers to a clean **Service-Oriented Architecture (SOA)**.
- **Geodesic Distance Measurement**: Real-time point-to-point calculation utilizing the **Haversine Formula** for perfect curved earth surface approximations.
- **Dynamic Buffer Analysis Polygon**: Geodesic buffer circle creation (radius mapping) mapped around map nodes.
- **State-Isolated UI & Gesture Handling**: Resolved overlapping mouse input states between map dragging (pan) and vertex placing (click) through a dynamic **Event Interception** system.
- **Adaptive Sidebar Layout**: Dynamic layout and panel hierarchy resizing built natively via code-behind, ensuring visual scalability without relying on Visual Studio's fragile Form Designer serialization.

---

## 🛠️ Software Engineering & Architecture

### 1. Eliminating Bloated Code-Behind (`Form1.cs`)
Standard WinForms applications often bundle API calls, business logic, and geometry rendering within `Form1.cs`. This PoC implements a strict **Separation of Concerns (SoC)** by spinning up specialized services:
- `MapDataService`: Manages data models (`DisasterPoint`) and asynchronous Mock API requests, ready for integration with official government real-time APIs (e.g., NCDR).
- `DisasterOverlayManager`: Exclusively dictates map markers/pins toggle, lifecycle, and rendering pipelines.
- `MapMeasurementService`: Holds pure geometry logic, coordinate computations, and line/polygon overlay draw operations.

### 2. Solving Input Conflict via Event Interception
In professional GIS platforms, **Left-Click & Drag** must seamlessly coexist with **Left-Click to Draw**. Standard WinForms control bindings crash or misfire due to ambiguous mouse states. 
By overriding `MouseDown` and `Up` deltas, this architecture establishes an execution barrier:
- Panning remains natively available via the Left Mouse Button.
- Measuring state hooks into the mouse pipeline safely, ensuring no input is swallowed and zero compilation errors occur from conflicting designer behaviors.

---

## 📐 Mathematical & Spatial Mechanics

Rather than leveraging external bulky geospatial engines, the geometry processing in this project runs via pure math optimization—leveraging background knowledge derived from low-level game engine and collision pipeline architecture.

### Great-Circle Distance (Haversine Formula)
To measure true geographical distance across the Earth's curved surface (WGS84 ellipsoid model), the system implements the Haversine formula instead of flat Cartesian Euclidean math:

$$\Delta lat = lat_2 - lat_1$$
$$\Delta lon = lon_2 - lon_1$$
$$a = \sin^2\left(\frac{\Delta lat}{2}\right) + \cos(lat_1) \cdot \cos(lat_2) \cdot \sin^2\left(\frac{\Delta lon}{2}\right)$$
$$c = 2 \cdot \text{atan2}\left(\sqrt{a}, \sqrt{1-a}\right)$$
$$d = R \cdot c$$

*(Where $R$ is Earth's mean radius $\approx 6,371,000$ meters)*

### Geodesic Buffer Analysis (Circle Projection)
Generating a 1,000-meter buffer zone on a Mercator projection without stretching distortion requires computing a polygon with localized trigonometric offsets. The system generates 36 precise vertices on the ellipsoidal sphere before handing the projection sequence to the GPU viewport overlay:

$$\Delta Lat = \frac{\text{Radius}}{6,371,000} \times \frac{180}{\pi}$$
$$\Delta Lng = \frac{\text{Radius}}{6,371,000 \times \cos\left(Lat_{\text{center}} \times \frac{\pi}{180}\right)} \times \frac{180}{\pi}$$

---

## 📂 Project Structure

```text
GisWinFormsNet8App/
│
├── Services/
│   ├── MapDataService.cs          # API communication & Mock data generation
│   ├── MapMeasurementService.cs   # Core spatial geometry & math logic
│   └── DisasterOverlayManager.cs  # Map Marker layout & Layer control
│
├── Models/
│   └── DisasterPoint.cs           # Pure structural data entity
│
├── Form1.cs                       # UI Shell (Pure View Router, completely lightweight)
└── Program.cs                     # Application Entry Point
