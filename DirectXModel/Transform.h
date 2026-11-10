//*********************************************************
// Transform.h
// Model world transform exposed to XAML/C#.
//*********************************************************

#pragma once
using namespace Windows::Foundation::Numerics;

namespace DirectXModel
{
	public ref class Transform sealed
	{
	public:
		Transform();
		property float3 Translation;
		// Euler angles in degrees, applied X then Y then Z.
		property float3 Rotation;
		property float3 Scale;

	internal:
		// World matrix = Scale * RotX * RotY * RotZ * Translation.
		DirectX::XMMATRIX GetWorldMatrix();
	};
}
