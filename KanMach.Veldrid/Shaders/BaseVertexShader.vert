#version 450

layout(set = 0, binding = 0) uniform ModelBuffer
{
    mat4 Model;
};

layout(set = 1, binding = 0) uniform ViewBuffer
{
    mat4 View;
};

layout(set = 1, binding = 1) uniform ProjectionBuffer
{
    mat4 Projection;
};



layout(location = 0) in vec3 Position;

void main()

    vec4 modelPosition = Model * vec4(Position, 1);
    vec4 viewPosition = View * modelPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
}