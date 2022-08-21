#version 450

struct Material {
    vec3 Diffuse;
    float Shininess;
    vec3 Specular;
};

layout (set = 0, binding = 4) uniform LightColor {
    vec3 fs_in_LightColor;
};
layout (set = 0, binding = 5) uniform AmbientColor {
    vec3 fs_in_AmbientColor;
};

layout (set = 0, binding = 6) uniform MaterialProperties {
    Material fs_in_Material;
};

layout (location = 0) in vec3 fs_in_FragPos;
layout (location = 1) in vec3 fs_in_Normal;
layout (location = 2) in vec3 fs_in_LightPos;

layout (location = 0) out vec4 fs_out_Color;

vec3 getDiffuseColor(vec3 normal, vec3 lightDir, vec3 lightColor) {
    float diffuse = max(dot(normal, lightDir), 0);
    return diffuse * lightColor;
}

vec3 getSpecularColor(vec3 normal, vec3 lightDir, vec3 viewDir, vec3 lightColor) {
    vec3 reflectDir = reflect(-lightDir, normal);
    float specular = pow(max(dot(viewDir, reflectDir), 0.0), fs_in_Material.Shininess);
    return fs_in_Material.Specular * specular * lightColor;
}

void main()
{
    vec3 ambientColor = fs_in_AmbientColor;
    vec3 objectColor = fs_in_Material.Diffuse;

    vec3 normal = normalize(fs_in_Normal);
    vec3 lightDir = normalize(fs_in_LightPos - fs_in_FragPos);

    vec3 diffuseColor = getDiffuseColor(normal, lightDir, fs_in_LightColor);

    vec3 viewDir = normalize(-fs_in_FragPos);
    vec3 specularColor = getSpecularColor(normal, lightDir, viewDir, fs_in_LightColor);

    fs_out_Color =  vec4((ambientColor + specularColor + diffuseColor) * objectColor, 1);
}

