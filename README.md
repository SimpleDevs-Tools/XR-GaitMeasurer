# XR-GaitMeasurer

_A tool to provide vertical gain to offset non-horizontal terrain in the real world. Primarily for XR devices (i.e. Quest systems). Previously called `GaitOscillationMeasurer`_.

In real-world circumstances, while it is optimal for XR studies to be conducted in lab spaces that guarantee flat or near-horizontal flooring, XR studies that take place outside are not guaranteed that same luxury. 

In some cases, real-world terrain that is presumedly flat may not necessarily be flat. To counter this, a vertical offset needs to be added to the XR camera. Otherwise, players might find themselves "falling" through the floor as they walk.

This package provides this offset functionality out-of-the-box.

**This package is intended for Quest devices (i.e. Quest 2, Quest 3, Quest Pro) or any XR HMD that comes with inside-out tracking!**

## Dependencies & Resources

* TextMeshPro
* XR Plugin Management
* [Meta All-in-One SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-all-in-one-sdk-269657)

## Purpose of this tool

When implemented into a project, the tool will require a phase where the player must naturally walk back and forth. The `GaitOscillationMeasurer` will measure the natural head bobble of the player's gait, creating a world-based reference for how much the head oscillates.

When the next stage is enabled, the system will detect any moment when the player's head starts to go out-of-bounds of this min-max range, relative to the world origin vertical position. If it detects any discrepancy, it will add an "offset" value to the `Camera Rig` of the player, thus forcing the system to remain within the natural gait oscillation of the player. The intended effect is for the player to not feel like they are falling through the virtual floor.

## Usage

1. Make sure that your Unity project is already fitted for XR.
2. Attach the `GaitOscillationMeasurer` component to any Game Object in the scene.
3. The `GaitOscillationMeasurer` component comes with several inspector references that need to be set:
    * `CameraRig`: the parent Transform for the XR rig. NOT the camera offset.
    * `HeadRef`: The Camera or GameObject that represents the player's head pose. Recommended to be `CenterEyeAnchor` for Quest systems.
    * `MinTextbox` (OPTIONAL) If you wish to display the min world height of the player's gait, then set a `TextMeshProUGUI` element to print the text here. Debug purposes primarily.
    * `MaxTextbox` (OPTIONAL) If you wish to display the max world height of the player's gait, then set a `TextMeshProUGUI` element to print the text here. Debug purposes primarily.
4. There are FOUR (4) stages of the Gait Oscillator: 
    * A **"Measurer"** stage where the player's head gait is measured.
    * A **"Corrector"** stage where the system will measure the appropriate offset that needs to be applied to prevent the player's camera from going out of range of the measured `Min Y` and `Max Y`. 
    * An **"Applier"** stage where the system will actually the offset to the `CameraRig`.
    * Not really a stage, but you can reset the oscillator via function call.

The ideal order of operations is:

1. Activate **Measurer** stage first.
2. Deactivate **Measurer**, then activate both **Corrector** and **Applier** stages.
3. At any time, the **Applier** stage can be toggled on or off.
4. If a reset is needed, then `Reset()` can be called.

### Measurer Stage

Needs to be called first before anything else. Several ways to activate this stage:

* `ToggleMeasurer()`: If called once, it will initialize the **Measurer** stage; the player must walk naturally to measure head gait. If called a second time, it will toggle the measurer off, thus caching the player's min-max gait heights.
* `StartMeasurer()`: If called, it will force the **Measurer** stage to activate.
* `EndMeasurer()`: If called, it will force the **Measurer** stage to end.

Outcome: `Min Y` and `Max Y` are updated.

### Corrector Stage

Can be called anytime the **Measurer** stage is completed. Several ways to activate this stage:

* `ToggleCorrector()`: If called once, it will initialize the **Corrector** stage; the required offset that needs to be applied to `CameraRig` is measured here. If called a second time, it will toggle the corrector off.
* `StartCorrector()`: If called, it will force the **Corrector** AND **Applier** stages to activate.
* `EndCorrector()`: If called, it will force the **Corrector** to end. However, it does NOT deactivate the **Applier** stage.

### Applier Stage

By default, this stage is also started when the **Corrector** stage is started too. This stage applies the height offset measured by the **Corrector** stage to the `CameraRig`. Several ways to activate this stage:

* `ToggleApplier()`: If called once, it will initialize the **Applier** stage; the player will have their height corrected via modifying the height of `CameraRig` based on the height offset measured by **Corrector**. If called a second time, it will toggle the applier off.
* `StartApplier()`: If called, it will force the **Applier** stage to activate.
* `EndApplier()`: If called, it will force the **Applier** stage to end.

**NOTE**: _You technically don't HAVE to have the **Applier** stage also activated alongside the **Corrector** stage. They can work separately. If you want to toggle the **Applier** stage on or off, call any of the functions above._

### Reset

If required, you can reset any cached height offsets and `Min Y` and `Max Y` by calling `Reset()`.