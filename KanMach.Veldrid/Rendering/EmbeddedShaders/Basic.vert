#version 450

layout (set = 0, binding = 0) uniform ModelBuffer
{
    mat4 Model;
};

layout (set = 0, binding = 1) uniform ViewBuffer
{
    mat4 View;
};

layout (set = 0, binding = 2) uniform ProjectionBuffer
{
    mat4 Projection;
};

layout (set = 0, binding = 3) uniform LightPos {
    vec3 vs_in_LightPos;
};

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normal;
layout (location = 2) in vec2 UV;

layout (location = 0) out vec3 vs_out_FragPos;
layout (location = 1) out vec3 vs_out_Normal;
layout (location = 2) out vec3 vs_out_LightPos;
layout (location = 3) out vec2 vs_out_TexturePos;

void main()
{ 
    gl_Position = Projection * View * Model * vec4(Position, 1);

    vs_out_FragPos = vec3(View * Model * vec4(Position, 1));
    vs_out_Normal = vec3(mat3(transpose(inverse(View * Model))) * Normal);
    vs_out_LightPos = vec3(View * vec4(vs_in_LightPos, 1));
    vs_out_TexturePos = UV;

}