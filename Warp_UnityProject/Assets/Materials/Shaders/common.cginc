#define UP float3(0,1,0)
#define RIGHT float3(1,0,0)
#define SPECULAR_DIRECTION float3(0, 0.7071, 0.7071)

fixed3 calculate_isometric_surround_light(float3 local_normal, float angle, fixed3 top_color, fixed3 left_color, fixed3 right_color)
{
    fixed rad_angle = angle * 3.1415 / 180;
    float3 n = normalize(mul(float4(local_normal, 0.0), unity_WorldToObject).xyz);
    float up_degree = max(0, dot(UP, n));
    float inverse_up_degree = sqrt(1 - up_degree * up_degree);

    fixed3 top = top_color;
    fixed3 surround = lerp(left_color, right_color, (dot(float3(cos(rad_angle), 0, sin(rad_angle)), n) + 1.0) * 0.5);

    return top * up_degree + surround * inverse_up_degree;
}

fixed3 calculate_rising_ambient_light(float4 local_pos, fixed3 bottom_color, fixed3 top_color, float high_point, float rise)
{
    float height = mul(unity_ObjectToWorld, local_pos).y;
    return lerp(bottom_color, top_color, pow(max(0, min(height / high_point, 1)), rise));
}

fixed3 calculate_fixed_light_specular(fixed3 world_pos, fixed angle, fixed3 specular_color, float shininess)
{
    fixed rad_angle = angle * 3.1415 / 180;
    fixed3 viewDirection = normalize(_WorldSpaceCameraPos - world_pos);
    return specular_color * pow(max(0.0, dot(reflect(-float3(0, sin(rad_angle), cos(rad_angle)), UP), viewDirection)), shininess);
}