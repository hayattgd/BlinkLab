#version 330 core

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;
layout (location = 2) in vec4 in_color;

uniform mat4 uProjection;

out vec2 frag_uv;
out vec4 frag_color;

void main() {
	frag_uv = in_uv;
	frag_color = in_color;
	gl_Position = uProjection * vec4(in_pos, 0.0, 1.0);
}
