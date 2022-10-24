#version 450

layout (set = 0, binding = 3) uniform Color {
    vec3 f_in_color;
};

layout(location = 0) out vec4 f_out_color;

void main()
{
    f_out_color =  vec4(f_in_color,1);
}