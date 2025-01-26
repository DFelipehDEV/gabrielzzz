shader_type canvas_item;
render_mode unshaded; // Credit: LiathTheCrow for original shader code

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
	COLOR = texture(SCREEN_TEXTURE, SCREEN_UV).rgba;
	float rgb_avg = (COLOR.r + COLOR.g + COLOR.b)/3.0;
	COLOR = round(r)

}