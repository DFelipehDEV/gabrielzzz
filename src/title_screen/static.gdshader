/*
Shader from Godot Shaders - the free shader library.
godotshaders.com/shader/VHS-and-CRT-monitor-effect

This shader is under CC0 license. Feel free to use, improve and 
change this shader according to your needs and consider sharing 
the modified result to godotshaders.com.
*/

shader_type canvas_item;

//*** IMPORTANT! ***/ 
// - If you are using this shader to affect the node it is applied to set 'overlay' to false (unchecked in the instepctor).
// - If you are using this shader as an overlay, and want the shader to affect the nodes below in the Scene hierarchy,
//   set 'overlay' to true (checked in the inspector).
// On Mac there is potentially a bug causing this to not work properly. If that is the case and you want to use the shader as an overlay
// change all "overlay ? SCREEN_TEXTURE : TEXTURE" to only "SCREEN_TEXTURE" on lines 129-140, and "vec2 uv = overlay ? warp(SCREEN_UV) : warp(UV);"
// to "vec2 uv = warp(SCREEN_UV);" on line 98.
uniform bool overlay = false;

uniform vec2 resolution = vec2(640.0, 480.0); // Set the number of rows and columns the texture will be divided in. Scanlines and grille will make a square based on these values

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform bool roll = true;
uniform float roll_speed = 8.0; // Positive values are down, negative are up
uniform float roll_size : hint_range(0.0, 100.0) = 15.0;
uniform float roll_variation : hint_range(0.1, 5.0) = 1.8; // This valie is not an exact science. You have to play around with the value to find a look you like. How this works is explained in the code below.
uniform float distort_intensity : hint_range(0.0, 0.2) = 0.05; // The distortion created by the rolling effect.

uniform float noise_opacity : hint_range(0.0, 1.0) = 0.4;
uniform float noise_speed = 5.0; // There is a movement in the noise pattern that can be hard to see first. This sets the speed of that movement.

uniform float static_noise_intensity : hint_range(0.0, 1.0) = 0.06;

uniform bool discolor = true; // Add a discolor effect simulating a VHS

// Used by the noise functin to generate a pseudo random value between 0.0 and 1.0
vec2 random(vec2 uv){
    uv = vec2( dot(uv, vec2(127.1,311.7) ),
               dot(uv, vec2(269.5,183.3) ) );
    return -1.0 + 2.0 * fract(sin(uv) * 43758.5453123);
}

// Generate a Perlin noise used by the distortion effects
float noise(vec2 uv) {
    vec2 uv_index = floor(uv);
    vec2 uv_fract = fract(uv);

    vec2 blur = smoothstep(0.0, 1.0, uv_fract);

    return mix( mix( dot( random(uv_index + vec2(0.0,0.0) ), uv_fract - vec2(0.0,0.0) ),
                     dot( random(uv_index + vec2(1.0,0.0) ), uv_fract - vec2(1.0,0.0) ), blur.x),
                mix( dot( random(uv_index + vec2(0.0,1.0) ), uv_fract - vec2(0.0,1.0) ),
                     dot( random(uv_index + vec2(1.0,1.0) ), uv_fract - vec2(1.0,1.0) ), blur.x), blur.y) * 0.5 + 0.5;
}

// Takes in the UV and warps the edges, creating the spherized effect
vec2 warp(vec2 uv){
	vec2 delta = uv - 0.5;
	float delta2 = dot(delta.xy, delta.xy);
	float delta4 = delta2 * delta2;
	
	return uv + delta * 0.0;
}

// Adds a black border to hide stretched pixel created by the warp effect
float border (vec2 uv){
	float radius = 0.0;
	radius = max(min(min(abs(radius * 2.0), abs(1.0)), abs(1.0)), 1e-5);
	vec2 abs_uv = abs(uv * 2.0 - 1.0) - vec2(1.0, 1.0) + radius;
	float dist = length(max(vec2(0.0), abs_uv)) / radius;
	float square = smoothstep(0.96, 1.0, dist);
	return clamp(1.0 - square, 0.0, 1.0);
}

void fragment()
{
	vec2 uv = overlay ? warp(SCREEN_UV) : warp(UV); // Warp the uv. uv will be used in most cases instead of UV to keep the warping
	vec2 text_uv = uv;
	vec2 roll_uv = vec2(0.0);
	float time = roll ? TIME : 0.0;
	
	// Create the rolling effect. We need roll_line a bit later to make the noise effect.
	// That is why this runs if roll is true OR noise_opacity is over 0.
	float roll_line = 0.0;
	if (roll || noise_opacity > 0.0)
	{
		// Create the areas/lines where the texture will be distorted.
		roll_line = smoothstep(0.3, 0.9, sin(uv.y * roll_size - (time * roll_speed) ) );
		// Create more lines of a different size and apply to the first set of lines. This creates a bit of variation.
		roll_line *= roll_line * smoothstep(0.3, 0.9, sin(uv.y * roll_size * roll_variation - (time * roll_speed * roll_variation) ) );
		// Distort the UV where where the lines are
		roll_uv = vec2(( roll_line * distort_intensity * (1.-UV.x)), 0.0);
	}
	
	vec4 text;
	if (roll)
	{
		// If roll is true distort the texture with roll_uv. The texture is split up into RGB to 
		// make some chromatic aberration. We apply the aberration to the red and green channels accorging to the aberration parameter
		// and intensify it a bit in the roll distortion.
		text.r = texture(SCREEN_TEXTURE, text_uv + roll_uv * 0.8).r;
		text.g = texture(SCREEN_TEXTURE, text_uv + roll_uv * 1.2).g;
		text.b = texture(SCREEN_TEXTURE, text_uv + roll_uv).b;
		text.a = 1.0;
	}
	else
	{
		// If roll is false only apply the aberration without any distorion. The aberration values are very small so the .1 is only 
		// to make the slider in the Inspector less sensitive.
		text.r = texture(SCREEN_TEXTURE, text_uv).r;
		text.g = texture(SCREEN_TEXTURE, text_uv).g;
		text.b = texture(SCREEN_TEXTURE, text_uv).b;
		text.a = 1.0;
	}
	
	float r = text.r;
	float g = text.g;
	float b = text.b;
	
	uv = warp(UV);
	
	// CRT monitors don't have pixels but groups of red, green and blue dots or lines, called grille. We isolate the texture's color channels 
	// and divide it up in 3 offsetted lines to show the red, green and blue colors next to each other, with a small black gap between.
	
	// Scanlines are the horizontal lines that make up the image on a CRT monitor. 
	// Here we are actual setting the black gap between each line, which I guess is not the right definition of the word, but you get the idea  

	// Apply the banded noise.
	if (noise_opacity > 0.0)
	{
		// Generate a noise pattern that is very stretched horizontally, and animate it with noise_speed
		float noise = smoothstep(0.4, 0.5, noise(uv * vec2(2.0, 200.0) + vec2(10.0, (TIME * (noise_speed))) ) );
		
		// We use roll_line (set above) to define how big the noise should be vertically (multiplying cuts off all black parts).
		// We also add in some basic noise with random() to break up the noise pattern above. The noise is sized according to 
		// the resolution value set in the inspector. If you don't like this look you can 
		// change "ceil(uv * resolution) / resolution" to only "uv" to make it less pixelated. Or multiply resolution with som value
		// greater than 1.0 to make them smaller.
		roll_line *= noise * clamp(random((ceil(uv * resolution) / resolution) + vec2(TIME * 0.8, 0.0)).x + 0.8, 0.0, 1.0);
		// Add it to the texture based on noise_opacity
		text.rgb = clamp(mix(text.rgb, text.rgb + roll_line, noise_opacity), vec3(0.0), vec3(1.0));
	}
	
	// Apply static noise by generating it over the whole screen in the same way as above
	if (static_noise_intensity > 0.0)
	{
		text.rgb += clamp(random((ceil(uv * resolution) / resolution) + fract(TIME)).x, 0.0, 1.0) * static_noise_intensity;
	}
	
	// Apply a black border to hide imperfections caused by the warping.
	// Also apply the vignette
	text.rgb *= border(uv);
	
	// Apply discoloration to get a VHS look (lower saturation and higher contrast)
	// You can play with the values below or expose them in the Inspector.
	float saturation = 0.5;
	float contrast = 1.2;
	if (discolor)
	{
		// Saturation
		vec3 greyscale = vec3(text.r + text.g + text.b) / 3.;
		text.rgb = mix(text.rgb, greyscale, saturation);
		
		// Contrast
		float midpoint = pow(0.5, 2.2);
		text.rgb = (text.rgb - vec3(midpoint)) * contrast + midpoint;
	}
	
	COLOR = text;
}