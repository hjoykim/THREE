const char* alphatest_fragment =R"(
#ifdef ALPHATEST

	if ( diffuseColor.a < ALPHATEST ) discard;

#endif
)";