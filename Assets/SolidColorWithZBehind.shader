
Shader "SolidColorWithZBehind" 
{
    Properties 
    {
        _Color1 ("Color1", Color) = (1,1,1,1)
    }
    SubShader 
    {
        Tags { "Queue" = "Geometry+1" }
        Pass 
        { 
            ZTest Greater
            Color [_Color1]
        }
    }
}
