const char* color_fragment =R"(
#ifdef USE_COLOR

	diffuseColor.rgb *= vColor;

#endif
)";