//Default Fragment Shader: simple single color fragment shader
//outputs the vertex color

#version 430

varying vec2 vTexCoord;

in vec3 FragPosition;
in vec3 FragNormal;
in vec3 FragColor;

uniform vec3 uLightColor;
uniform vec3 uLightPosition;
uniform vec3 uViewPos;
uniform float uLightIntensity;

layout(binding = 0) uniform sampler2D uSampler;

void main(void) {

   //precalculate variables, all vectors are facing towards the FragPosition,  
   vec3 ViewVector = (uViewPos - FragPosition);
   vec3 LightVector = (uLightPosition - FragPosition);
   vec3 HalfVector = (ViewVector + LightVector);
   vec3 LightDir = normalize(LightVector);
   vec3 HalfDir = normalize(HalfVector);
   float specularValue = 0.0f;
   float ambientValue = 0.1f;
   float shininess = 32.0f;
   float distanceFromLight = pow(length(LightVector), 2);
   float lambertianValue = max(dot(LightDir, FragNormal), 0.0f);
   
   //Set material base color
   vec3 newVertexColor = vec3(1.0f, 1.0f, 1.0f);
   
   //thanks to https://stackoverflow.com/questions/14978986/find-out-if-gl-texture-2d-is-active-in-shader
   //for the idea for the single white pixel sampler
   newVertexColor *= texture(uSampler, vTexCoord);
   newVertexColor *= FragColor;
   
   if (lambertianValue > 0.0f) {
      float specularAngle = max(dot(HalfDir, FragNormal), 0.0f);
      specularValue = pow(specularAngle, shininess);
   }
   
   vec3 ambient = vec3(ambientValue);
   
   vec3 diffuse = lambertianValue * uLightColor * uLightIntensity / distanceFromLight;

   vec3 specular = specularValue * uLightColor * uLightIntensity / distanceFromLight;

   //apply lighting
   newVertexColor *= (ambient + diffuse + specular);
   
   //do gamma correction for sRGB color space
   newVertexColor = pow(newVertexColor, vec3(1.0f / 2.2f));
   
   //set final fragment color
   gl_FragColor = vec4(newVertexColor, 1.0f);
}