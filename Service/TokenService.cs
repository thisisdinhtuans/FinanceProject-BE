using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        //tại sao đầu vào là IConfiguration? Vì nó cần phải đọc nội dung từ appsettings.json
        public TokenService(IConfiguration config)
        {
            _config=config;
            //sử dụng mã hóa Encoding và chuyển sang UTF8. Lý do sử dụng mã hóa là vì phải biến nó thành bytes nên nó sẽ không chấp nhận như một chỗi thống thường
            //đó là key, nếu ai đó có key của bạn, họ có thể tạo mã thông báo vì JWT tự nó phụ thuộc vào khóa kỹ tự thực tế của ứng dụng
            //SigningKey phải đủ dài không thì sẽ lỗi 
            _key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }
        public string CreateToken(AppUser user)
        {
            //nhận dạng người dùng cần email và username
            var claims=new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };
            //thông tin xác thực mới, tiếp tục sẽ chuyển khóa và khóa này(_key) là những thứ bạn đã chỉ định trong ứng dụng và chuyển sang thuật toán bảo mật(SecurityAlgorithms) và đây là dạng mã hóa mà chúng ta muốn sử dụng nên chuyển sang HmacSha512Signature
            var creds=new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            //Nơi thực sự tạo mã thống báo(Create token), sẽ thống báo dưới dạng đối tượng 
            var tokenDescriptor=new SecurityTokenDescriptor
            {
                //Subject như 1 chiếc ví, nó bao gồm email và name(vẽ ở trong vở)
                Subject=new ClaimsIdentity(claims),
                //Ngày hết hạn, giới hạn thời gian tồn tại
                Expires=DateTime.Now.AddDays(7),
                //không muốn nó tồn tại mãi mãi, bởi vì ai đó có token hoặc bị đánh cắp nên chúng ta cần SigningCredentials
                SigningCredentials=creds,
                Issuer=_config["JWT:Issuer"],
                Audience=_config["JWT:Audience"]
            };
            //TokenHandler là 1 phương pháp tạo ra mã thông báo thực sự 
            var tokenHandler=new JwtSecurityTokenHandler();
            //token Handler là một JwtSecurityTokenHandler()
            //đầu vào của CreateToken là SecurityTokenDescriptor đã tạo ở trên
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}