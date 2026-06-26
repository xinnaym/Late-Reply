shader_type canvas_item;

uniform float scanline_intensity : hint_range(0.0, 1.0) = 0.3;
uniform float noise_intensity : hint_range(0.0, 1.0) = 0.2;
uniform float color_bleed : hint_range(0.0, 1.0) = 0.15;
uniform float vignette_strength : hint_range(0.0, 1.0) = 0.3;
uniform float flicker_intensity : hint_range(0.0, 1.0) = 0.1;
uniform float time_speed : hint_range(0.0, 5.0) = 1.0;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

float random(vec2 co) {
	return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
	vec2 uv = SCREEN_UV;
	float time = TIME * time_speed;
	
	// Scanlines
	float scanline = sin(uv.y / SCREEN_PIXEL_SIZE.y * 1.5 + time * 3.0) * 0.5 + 0.5;
	scanline = pow(scanline, 1.5) * scanline_intensity;
	
	// Noise
	float noise = random(uv + fract(time)) * noise_intensity;
	
	// Color bleed (chromatic aberration)
	float bleed_offset = color_bleed * 0.003;
	vec4 color_r = textureLod(SCREEN_TEXTURE, uv + vec2(bleed_offset, 0.0), 0.0);
	vec4 color_g = textureLod(SCREEN_TEXTURE, uv, 0.0);
	vec4 color_b = textureLod(SCREEN_TEXTURE, uv - vec2(bleed_offset, 0.0), 0.0);
	vec4 color = vec4(color_r.r, color_g.g, color_b.b, color_g.a);
	
	// Vignette
	vec2 vignette_uv = uv * (1.0 - uv);
	float vignette = vignette_uv.x * vignette_uv.y * 15.0;
	vignette = pow(vignette, vignette_strength);
	
	// Flicker
	float flicker = 1.0 - (random(vec2(time * 0.1, 0.0)) * flicker_intensity);
	
	// Apply effects
	color.rgb *= vignette;
	color.rgb += noise;
	color.rgb -= scanline * 0.15;
	color.rgb *= flicker;
	
	// Slight desaturation for PSX feel
	float gray = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	color.rgb = mix(color.rgb, vec3(gray), 0.15);
	
	// Slight warmth
	color.r *= 1.05;
	color.g *= 0.98;
	
	COLOR = color;
}
