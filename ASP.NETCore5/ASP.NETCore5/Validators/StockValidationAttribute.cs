using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ASP.NETCore5.Models;

namespace ASP.NETCore5.Validators
{
    public class StockValidationAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // 1. Lấy đối tượng Product hiện tại mà thuộc tính này đang được áp dụng
        Product product = (Product)validationContext.ObjectInstance;

        // 2. Thực hiện logic kiểm tra
        if (product.Quantity < 5)
        {
            // Trả về lỗi nếu số lượng nhỏ hơn 5
            return new ValidationResult("Product stock has minimum 5 units.");
        }

        // 3. Trả về thành công nếu kiểm tra qua
        return ValidationResult.Success;
    }

    }
}
