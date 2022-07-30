# COMP30019 Assignment 1 - Ray Tracer
This is your README.md... you should write anything relevant to your implementation here.

Please ensure your student details are specified below (*exactly* as on UniMelb records):

**Name:** Rui Guo \
**Student Number:** 993228 \
**Username:** reguo \
**Email:** reguo@student.unimelb.edu.au

## Completed stages

Tick the stages bellow that you have completed so we know what to mark (by editing README.md). At most **six** marks can be chosen in total for stage three. If you complete more than this many marks, pick your best one(s) to be marked!

<!---
Tip: To tick, place an x between the square brackes [ ], like so: [x]
-->

##### Stage 1

- [x] Stage 1.1 - Familiarise yourself with the template
- [x] Stage 1.2 - Implement vector mathematics
- [x] Stage 1.3 - Fire a ray for each pixel
- [x] Stage 1.4 - Calculate ray-entity intersections
- [x] Stage 1.5 - Output primitives as solid colours

##### Stage 2

- [x] Stage 2.1 - Diffuse materials
- [x] Stage 2.2 - Shadow rays
- [x] Stage 2.3 - Reflective materials
- [x] Stage 2.4 - Refractive materials
- [x] Stage 2.5 - The Fresnel effect
- [x] Stage 2.6 - Anti-aliasing

##### Stage 3

- [ ] Option A - Emissive materials (+6)
- [ ] Option B - Ambient lighting/occlusion (+6)
- [ ] Option C - OBJ models (+6)
- [x] Option D - Glossy materials (+3)
- [ ] Option E - Custom camera orientation (+3)
- [x] Option F - Beer's law (+3)
- [ ] Option G - Depth of field (+3)

Glossy Materials is implemented by combining diffuse color at the material as well as a sclaed reflactive color, with the factor of 0.5, to mimick 
the reflective nature of glossy materials.
Beer's law is implemtned by deducting a factor of 0.05*Color channel of material from each of the color channels in order to simulate absorbance of light depending on color. 



## Final scene render

Be sure to replace ```/images/final_scene.png``` with your final render so it shows up here:

![My final render](/images/final_scene.png)

This render took **17** minutes and **32** seconds on my PC.

I used the following command to render the image exactly as shown:

```
dotnet run -- -f tests/final_scene.txt -o final_scene.png -x 4
```

## Sample outputs

We have provided you with some sample tests located at ```/tests/*```. So you have some point of comparison, here are the outputs our ray tracer solution produces for given command line inputs (for the first two stages, left and right respectively):

###### Sample 1
```
dotnet run -- -f tests/sample_scene_1.txt -o images/sample_scene_1.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_1_s1.png" />
  <img src="/images/sample_scene_1_s2.png" /> 
</p>

###### Sample 2

```
dotnet run -- -f tests/sample_scene_2.txt -o images/sample_scene_2.png -x 4
```
<p float="left">
  <img src="/images/sample_scene_2_s1.png" />
  <img src="/images/sample_scene_2_s2.png" /> 
</p>

## References

*You must list any references you used!*

To get you started, here is some good reading material:

Working through a ray tracer, from the head of the xbox games studio: https://www.linkedin.com/pulse/writing-simple-ray-tracer-c-matt-booty/

*Ray Tracing in a Weekend*: https://raytracing.github.io/

Great walkthrough of some of the basic maths: https://blog.scottlogic.com/2020/03/10/raytracer-how-to.html

Scratchapixel: intro to ray tracing: https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-ray-tracing/how-does-it-work

https://www.scratchapixel.com/code.php?id=10&origin=/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes

https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/ray-triangle-intersection-geometric-solution

https://graphics.stanford.edu/courses/cs148-10-summer/docs/2006--degreve--reflection_refraction.pdf

https://www.scratchapixel.com/code.php?id=13&origin=/lessons/3d-basic-rendering/introduction-to-shading