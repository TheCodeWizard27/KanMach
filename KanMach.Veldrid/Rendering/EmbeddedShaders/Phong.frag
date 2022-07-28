#version 450

layout (set = 0, binding = 4) uniform LightColor {
    vec3 fs_in_LightColor;
};

layout (location = 0) in vec3 fs_in_FragPos;
layout (location = 1) in vec3 fs_in_Normal;
layout (location = 2) in vec3 fs_in_LightPos;

layout (location = 0) out vec4 fs_out_Color;

vec3 getDiffuseColor(vec3 normal, vec3 lightDir, vec3 lightColor) {
    float diffuse = max(dot(normal, lightDir), 0);
    return diffuse * lightColor;
}

// Everything needs to be in view space.
vec3 getSpecularColor(vec3 normal, vec3 lightDir, vec3 viewDir, vec3 lightColor) {
    float specularStrength = 0.5;

    vec3 reflectDir = reflect(-lightDir, normal);
    float specular = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    return specularStrength * specular * lightColor;
}

void main()
{
    vec3 ambientColor = vec3(0.4, 0.4, 0.4);
    vec3 objectColor = vec3(0,0.4,0.8);

    vec3 normal = normalize(fs_in_Normal);
    vec3 lightDir = normalize(fs_in_LightPos - fs_in_FragPos);

    vec3 diffuseColor = getDiffuseColor(normal, lightDir, fs_in_LightColor);

    vec3 viewDir = normalize(-fs_in_FragPos);
    vec3 specularColor = getSpecularColor(normal, lightDir, viewDir, fs_in_LightColor);

    fs_out_Color =  vec4((ambientColor + diffuseColor + specularColor) * objectColor, 1);
}

