# Virtual Ray Tracer in Virtual Reality

Virtual Ray Tracer is a Unity application that visualizes ray tracing. The application shows a scene with a camera, lights and objects. Rays slowly shoot from the camera and interact with the lights and objects in the scene. Users can change the settings of the ray tracer and material properties of the objects and see how this affects the rays traced in real-time. The application comes with a number of scenes that introduce the user to settings and controls as well as various ray tracing concepts.

<p align="center">
  <img src="https://github.com/user-attachments/assets/6aa357aa-e018-4073-8ba3-b37308687f7d" width=600/>
<p/>

This repository is only for the VR version of Virtual Ray Tracer. Other versions can be found in the [main repository](https://github.com/wezel/Virtual-Ray-Tracer).

## Building the Application

As a prerequisite, you need a [Unity 2021.3.12f1 LTS](https://unity3d.com/unity/qa/lts-releases) release. To build the application, open the `Unity` folder with Unity, navigate to `File > Build Settings`, select Windows for PCVR or Android for Standalone VR, then press 'build'. For more information on building Unity applications see the [Unity Manual page](https://docs.unity3d.com/Manual/BuildSettings.html).

## Implementation

There are three classes at the core of the application: the `SceneManager`, the `RayManager` and the `UnityRayTracer`. The `SceneManager` handles the scene data consisting of a camera, lights and objects. The `UnityRayTracer` takes this scene data and produces a set of rays. The `RayManager` is in charge of visualizing these rays.

#### Scene Management

Almost all scene data relevant to the ray tracer is stored in standard Unity components. For example, the material properties of an object are stored in a `Material` component. To mark certain `GameObject`s in the Unity scene as a camera, light or mesh to be sent to the ray tracer, a custom `RTCamera`, `RTLight` or `RTMesh` component should be attached. These components each provide fields with getters and setters for easy access to properties needed for ray tracing. Beyond that, the `RTLight` and `RTMesh` components are not much more than tags to indicate to the `SceneManager` that the objects they are attached to should be sent to the ray tracer. The `RTCamera` component is slightly more complex.  It does not rely on an attached Unity `Camera` component and needs to store its own data. It also has code for drawing the camera in the scene.

At the start of a scene, the `SceneManager` finds all instances of `RTCamera`, `RTLight` and `RTMesh` components and stores them in one `RTScene` object. This `RTScene` object then serves as the input to the ray tracer. The `SceneManager` also handles the selection and deselection of objects in the scene as well as object creation and deletion.

#### Ray Tracing

The ray tracer is implemented in the `UnityRayTracer` class. The ray tracing is done by using Unity's built in `Physics.Raycast` function to cast rays and determine object intersections. This function casts a ray from a given point and returns information about the first object with a `Collider` component that it intersects. Each object with an `RTMesh` component has such a `Collider` component, so the ray tracer is aware of them.

Most of the ray tracer code is relatively straightforward if you are familiar with ray tracing. It closely matches the code of the ray tracer used in the Computer Graphics course taught at the University of Groningen. The most important difference is that main output of the ray tracer is rays instead of images.

The rays are represented with simple `RTRay` objects that store the origin, direction and length of the ray. Because ray tracing follows a recursive pattern with one ray potentially resulting in many more rays being cast, we store the rays in a tree. Each recursive call to the `Trace` function builds up a `TreeNode<RTRay>` object which is then added as a child of the caller's tree. The output of the ray tracer's `Render` function is then a `List<TreeNode<RTRay>>`, one `TreeNode<RTRay>` for each pixel.

#### Ray Visualization

The `RayManager` takes this list of ray trees and draws them in the scene. Most of the drawing code lives in the `RayObject` class. `RayObject`s take in an `RTRay` and they position, orient and scale a cylinder to match the ray's origin, direction and length. The `RayManager` simply manages these `RayObjects` by providing them the rays from the ray trees produced by the ray tracer. When animating the rays, the `RayManager` also informs each `RayObject` how long it needs to be. By increasing the length a bit each frame the `RayManager` can achieve the effect of rays slowly shooting from the camera into the scene. This animation is done in a recursive fashion, so first all rays at the root of their ray tree are extended, then their children, and so on.

Ideally, each `RayObject` would be paired with one `RTRay` for its entire lifetime. However, this becomes a problem when the user changes something about the scene and a new list of ray trees is produced by the ray tracer. The simplest thing to do then would be to destroy all existing `RayObject`s and instantiate new ones for the new ray trees. The problem is that this becomes too slow when the user changes a value in the scene continuously (e.g. by dragging around an object) and there are new rays being traced every frame. Instead, it is possible to reuse existing `RayObjects` by providing them with a new `RTRay` and deactivating those we do not need. This approach of having a set of objects and activating and deactivating them instead of instantiating and destroying them is called object pooling. It is implemented in the `RayObjectPool` class.

#### Further Details

This covers all the most important components of the application, but there are many more small details. Please look through the source code if you want to know more, we strive to having everything well documented.

## About us

Virtual Ray Tracer was created by Chris van Wezel and Willard Verschoore as a graduation project for their Computing Science Bachelor's degree programme at the University of Groningen. The project was proposed and supervised by Jiri Kosinka and Steffen Frey in the [SVCG research group](https://www.cs.rug.nl/svcg/Main/HomePage). The application was built to aid students of the Computer Graphics course at the University of Groningen by providing them with an interactive introduction to the principles of ray tracing. The corresponding paper is published as an educational paper at the Eurographics 2022 conference and the EG Digital Library: [Virtual Ray Tracer](https://diglib.eg.org/handle/10.2312/eged20221045). It was selected as one of the best educational papers at the conference, which lead to an extended paper which can be found at: [Virtual Ray Tracer 2.0](https://doi.org/10.1016/j.cag.2023.01.005).

## Future

The application is under active development and we hope that you will contribute to Virtual Ray Tracer, too; read on.

## License

The application is released under the MIT license. Therefore, you may use and modify the code as you see fit. If you use or build on the application we would appreciate it if you cited this repository and the Eurographics 2022 paper. As we are still working on the application ourselves, we would also like to hear about any improvements you may have made.

## Contact

Any questions, bug reports or suggestions can be created as an issue on this repository. Alternatively, please contact [Jiri Kosinka](http://www.cs.rug.nl/svcg/People/JiriKosinka).

## Papers and Theses

##### Papers:

[C.S. van Wezel, W.A. Verschoore de la Houssaije, S. Frey, and J. Kosinka: Virtual Ray Tracer 2.0, Special Section on EG2022 Edu Best Papers.](https://doi.org/10.1016/j.cag.2023.01.005)

[W.A. Verschoore de la Houssaije, C.S. van Wezel, S. Frey, and J. Kosinka: Virtual Ray Tracer, Eurographics 2022 Education Papers.](https://diglib.eg.org/handle/10.2312/eged20221045)

##### Theses:

[W.A. Verschoore de la Houssaije: A Virtual Ray Tracer, BSc thesis, University of Groningen, 2021.](http://fse.studenttheses.ub.rug.nl/24859)

[C.S. van Wezel: A Virtual Ray Tracer, BSc thesis, University of Groningen, 2022.](http://fse.studenttheses.ub.rug.nl/26455)

[J. van der Zwaag: Virtual Ray Tracer: Distribution Ray Tracing, BSc Thesis, University of Groningen, 2022.](https://fse.studenttheses.ub.rug.nl/27881)

[B. Yilmaz: Acceleration data structures for Virtual Ray Tracer, BSc Thesis, University of Groningen, 2022.](https://fse.studenttheses.ub.rug.nl/27838)

[PJ. Blok: Gamification of Virtual Ray Tracer, BSc Thesis, University of Groningen, 2022.](https://fse.studenttheses.ub.rug.nl/27596)

[R. Rosema: Adapting Virtual Ray Tracer to a Web and Mobile Application, BSc Thesis, University of Groningen, 2022.](https://fse.studenttheses.ub.rug.nl/27894)

[A. Aaen: Virtual Ray Tracer in VR, BSc thesis, University of Groningen, 2021.](https://fse.studenttheses.ub.rug.nl/33355/)

Further documents will appear here in due course.
