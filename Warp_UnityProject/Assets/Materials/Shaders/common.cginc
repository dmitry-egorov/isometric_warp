#define UP float3(0,1,0)
#define RIGHT float3(0.7071,0,0.7071)
#define SPECULAR_DIRECTION float3(-0.57735, 0.57735, 0.57735)
#define VIEW_DIRECTION float3(0.57735, 0.57735, -0.57735)
            
			
fixed3 calculate_isometric_surround_light(float3 local_normal, fixed3 top_color, fixed3 left_color, fixed3 right_color)
{
    float3 n = normalize(mul(float4(local_normal, 0.0), unity_WorldToObject).xyz);
    float up_degree = dot(UP, n);
    float inverse_up_degree = sqrt(1 - up_degree * up_degree);

    fixed3 top = top_color * max(0, up_degree);
    fixed3 surround = lerp(left_color, right_color, (dot(RIGHT, n) + 1.0) * 0.5) * inverse_up_degree;

    return top + surround;
}

fixed3 calculate_rising_ambient_light(float4 local_pos, fixed3 bottom_color, fixed3 top_color, float high_point, float rise)
{
    float height = mul(unity_ObjectToWorld, local_pos).y;
    return lerp(bottom_color, top_color, pow(max(0, min(height / high_point, 1)), rise));
}

fixed3 calculate_fixed_light_specular(fixed3 world_pos, fixed3 specular_color, fixed3 light_color, float shininess)
{
    fixed3 viewDirection = normalize(_WorldSpaceCameraPos - world_pos);
    return specular_color * light_color * pow(max(0.0, dot(reflect(-SPECULAR_DIRECTION, UP), viewDirection)), shininess);
}