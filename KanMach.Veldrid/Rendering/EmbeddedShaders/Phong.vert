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

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normal;

layout (location = 0) out vec3 v_out_normal;
layout (location = 1) out vec3 v_out_position;
layout (location = 2) out vec3 v_out_lightPos;

void main()
{
    mat4 pvm = Projection * View * Model;
    
    gl_Position = pvm * vec4(Position, 1);
    v_out_normal = Normal;
    v_out_position = vec3(Model * vec4(Position, 1.0));
    v_out_lightPos = vec3(1,0,0);

}