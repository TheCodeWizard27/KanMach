#version 450


layout (location = 0) in vec3 v_out_normal;
layout (location = 1) in vec3 v_out_position;
layout (location = 2) in vec3 v_out_lightPos;

layout (location = 0) out vec4 f_out_color;

void main()
{
    vec3 lightColor = vec3(1);

    vec3 norm = normalize(v_out_normal);
    vec3 lightDir = normalize(v_out_lightPos - v_out_position);

    // Diffuse calculation
    vec3 ambient = vec3(0.8, 0.8, 0.8);

    float diffuse = max(dot(norm, v_out_lightPos), 0);
    vec3 diffuseColor = diffuse * lightColor;

    // Specular calculation
    float specularStrength = 0.5;

    vec3 viewDir = normalize(vec3(0,0,0) - v_out_position);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColor;

    vec3 objectColor = vec3(0,0.4,0.8);

    f_out_color =  vec4((ambient + diffuseColor + specular) * objectColor, 1);
}