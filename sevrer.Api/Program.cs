//config jwt auth
builder.Services.AddAuthentication(options =>
{
    //scheme used to authenticate incoming requests (verify who the user is)
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // scheme used when a request fails authentication for JWT, this usually results in a 401 Unauthorized response
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//config jwt bearer options
.AddJwtBearer(options =>
{   //loads the jwt section appsettings.config
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    //convert secret key into byte array
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
    options.TokenValidationParameters = new TokenValidationParameters
    {   //iss matching
        ValidateIssuer = true,
        //aud matching
        ValidateAudience = true,
        //token isn't expired
        ValidateLifetime = true,
        //token was assigned with expected key
        ValidateIssuerSigningKey = true,
        //valid issuer from my configs
        ValidIssuer = jwtSettings["Issuer"],
        //valid ausience form my configs
        ValidAudience = jwtSettings["Audience"],
        //secret key
        used to verify the token’s signature
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
//adding password hasher service
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();