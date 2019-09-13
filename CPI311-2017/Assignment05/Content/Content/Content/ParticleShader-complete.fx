// ParticleShader.fx, effect sample
// OGATA Kaoru

float4x4 World, ViewProj;
float4x4 CamIRot;	// カメラの逆回転の行列

// 頂点シェーダからの戻り値は複数のセマンティクスなので
// 構造体で定義する
struct VS_OUTPUT {
	float4 Pos: POSITION;		// 頂点座標位置
	float2 UV: TEXCOORD0;		// UV
	float4 Col: COLOR0;			// 頂点の色
};

// 戻り値を複数要素にしたいので、VS_OUTPUT。
VS_OUTPUT vtxSh(
	float4 inPos: POSITION, 
	float2 inUV: TEXCOORD0, 
	float4 inPPos: POSITION1, 
	float4 inParam: POSITION2
) {
	VS_OUTPUT Out;
	float4 pp = inPos;
	//// ↓をコメントすると、カメラを向かないポリゴンになる
	pp = mul(pp, CamIRot);
	// ポリゴンをスケールする
	pp.xyz = pp.xyz * sqrt(inParam.x);
	// 位置を動かす
	pp += inPPos;
	float4 pos = mul(pp, World);		// ワールド空間に
	Out.Pos = mul(pos, ViewProj);		// 投影空間に
	Out.UV = inUV;
	// 色合いの調整
	Out.Col = 1-inParam.x/inParam.y;
	return Out;
}

texture2D Texture;
sampler texImage0: register(s0) = sampler_state {
    Texture = <Texture>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

// ピクセルシェーダを定義する
// 戻り値はCOLORセマンティクス
float4 pxlSh(
	VS_OUTPUT In
) : COLOR {
	float4 color = 0;
	color = tex2D(texImage0, In.UV);
	color *= In.Col;

	return color;
}

technique particle {
	pass P0 {
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlSh();
	}
}
