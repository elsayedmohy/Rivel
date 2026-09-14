global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.Extensions.Options;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Diagnostics;
global using FluentValidation;
// ************************************************

global using System.Text;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;

// ************************************************

global using RiverLine.Api;
global using RiverLine.Api.Models.Enums;
global using RiverLine.Api.Models.Entities;
global using RiverLine.Api.Models.Dtos.Auth;
global using RiverLine.Api.Data;
global using RiverLine.Api.Services.Auth;
global using RiverLine.Api.Settings;
global using RiverLine.Api.Middleware;