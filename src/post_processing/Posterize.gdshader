shader_type canvas_item;
render_mode unshaded;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
uniform int posterization_levels : hint_range(2, 256) = 16;

void fragment() {
    vec4 color = texture(SCREEN_TEXTURE, SCREEN_UV).rgba;
    
    // Posterization calculation
    float levels = float(posterization_levels);
    color.rgb = floor(color.rgb * levels) / levels;
    
    COLOR = vec4(color.rgb, color.a); // Preserve alpha
}
