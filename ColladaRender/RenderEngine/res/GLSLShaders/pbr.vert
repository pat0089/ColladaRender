//PBR Vertex Shader: 
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
    vTexCoord = aVertexTexCoord;
    FragPosition = vec3(uModel * vec4(aVertexPosition, 1.0));
    FragNormal = mat3(uModel) * aVertexNormal;
    FragColor = aVertexColor;
    gl_Position = vec4(FragPosition, 1.0f) * uView * uProjection;

}
