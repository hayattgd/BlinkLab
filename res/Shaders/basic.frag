#version 330 core

in vec2 frag_uv;
in vec4 frag_color;

out vec4 outColor;

uniform sampler2D uTexture;

void main() {
	outColor = frag_color * texture(uTexture, frag_uv);
}
