//Default Vertex Shader: 
//takes in all of the attribute values for each vertex and sends the appropriate data to the fragment shader 
//sets vertex position in the world space 
#version 430

layout (location = 0) in vec3 aVertexPosition;
layout (location = 1) in vec3 aVertexNormal;
layout (location = 2) in vec2 aVertexTexCoord;
layout (location = 3) in vec3 aVertexColor;

varying vec2 vTexCoord;

out vec3 FragPosition;
out vec3 FragNormal;
out vec3 FragColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main(void) {
    gl_Position = vec4(aVertexPosition, 1.0f) * uView * uModel * uProjection;
    FragPosition = vec3(vec4(aVertexPosition, 1.0f) * uModel);
    FragNormal = aVertexNormal * mat3(transpose(inverse(uModel)));
    vTexCoord = aVertexTexCoord;
    FragColor = aVertexColor;
}
