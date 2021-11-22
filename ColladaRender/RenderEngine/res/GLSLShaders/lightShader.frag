//Default Fragment Shader: simple single color fragment shader
//outputs the vertex color

#version 430

varying vec2 vTexCoord;

in vec3 FragPosition;
in vec3 FragNormal;
in vec3 FragColor;

uniform sampler2D uSampler;

//uniform vec3 uLightColor;
//uniform vec3 uLightPosition;
//uniform vec3 uViewPos;
//uniform float uLightIntensity;

void main(void) {

    //get material base color
    vec3 newVertexColor = vec3(1.0f, 1.0f, 1.0f);
    //newVertexColor *= texture(uSampler, vTexCoord);
    //newVertexColor *= FragColor;

    //ambient lighting
    //float ambientValue = uLightIntensity;
    //vec3 ambient = ambientValue * uLightColor;

    //get normalized light directon for diffuse and specular settings
    //vec3 norm = normalize(Normal);
    //vec3 lightDirection = normalize(uLightPosition - FragPosition);

    //diffuse lighting
    //float diffuseValue = max(dot(norm, lightDirection), 0.0f);
    //vec3 diffuse = diffuseValue * uLightColor;

    //specular lighting
    //float specStrength = 0.6f;
    //vec3 viewDirection = normalize(uViewPos - FragPosition);
    //vec3 reflectDirection = reflect(-lightDirection, norm);
    //float spec = pow(max(dot(viewDirection, reflectDirection), 0.0f), 32);
    //vec3 specular = specStrength * spec * uLightColor;

    //set final fragment color
    //newVertexColor *= (ambient + diffuse + specular);
    gl_FragColor = vec4(newVertexColor, 1.0f);
}