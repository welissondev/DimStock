﻿using DimStock.AuxilyTools.AuxilyClasses;

namespace DimStock.Models
{
    public class ProductValidationModel
    {
        public static bool ValidateToInsert(ProductModel product)
        {
            var validationStatus = false;

            if (product.InternalCode == string.Empty)
            {
                MessageNotifier.Show("Informe o código interno " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.Description == string.Empty)
            {
                MessageNotifier.Show("Informe a descrição " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.Category.Id == 0)
            {
                MessageNotifier.Show("Selecione a categoria " +
                "do produto!", "Não Selecionada", "?");

                return validationStatus;
            }

            if (product.CostPrice == 0.00)
            {
                MessageNotifier.Show("Informe o preço de custo " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.SalePrice == 0.00)
            {
                MessageNotifier.Show("Informe o preço de venda " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.CostPrice < 0.00)
            {
                MessageNotifier.Show("O preço de custo " +
                "do produto não pode ser negativo!",
                "Não Permitido", "?");

                return validationStatus;
            }

            if (product.SalePrice < 0.00)
            {
                MessageNotifier.Show("O preço de venda " +
                "do produto não pode ser negativo!",
                "Não Permitido", "?");

                return validationStatus;
            }

            if (product.CostPrice > product.SalePrice)
            {
                MessageNotifier.Show("O preço de custo do produto " +
                "não pode ser maior que o preço de venda!",
                "Não Permitido", "?");

                return validationStatus;
            }

            if (product.CostPrice == product.SalePrice)
            {
                MessageNotifier.Show("O preço de custo " +
                "do produto não pode ser igual ao preço " +
                "de venda!", "Não Permitido", "?");

                return validationStatus;
            }

            if (product.Stock.Min > product.Stock.Max)
            {
                MessageNotifier.Show("O estoque mínimo do produto " +
                "não pode ser maior que o estoque máximo!",
                "Não permitido", "?");

                return validationStatus;
            }

            if (product.Stock.Min > 0 && product.Stock.Min == product.Stock.Max)
            {
                MessageNotifier.Show("O estoque mínimo do produto " +
                "não pode ser igual ao estoque máximo!",
                "Não permitido", "?");

                return validationStatus;
            }

            return validationStatus = true;
        }

        public static bool ValidateToDelete(ProductModel product)
        {
            var validationStatus = false;

            if (product.Id == 0)
            {
                MessageNotifier.Show("Selecione o produto " +
                "para excluir!", "Não Selecionado", "?");

                return validationStatus;
            }

            if (product.CheckRegisterStatus() == false)
            {
                MessageNotifier.Show("Esse registro já foi " +
                "excluido, atualize a lista de produtos!",
                "Atualize a Lista", "?");

                return validationStatus;
            }

            if (product.GetQuantityInStock() > 0)
            {
                MessageNotifier.Show("Não é possível deletar " +
                "esse produto, porque ele possui entradas " +
                "no estoque!", "Não Permitido", "?");

                return validationStatus;
            }

            if (product.GetQuantityInStock() < 0)
            {
                MessageNotifier.Show("Não é possível deletar " +
                "esse produto, porque ele possui saídas " +
                "no estoque!", "Não Permitido", "?");

                return validationStatus;
            }

            if (MessageNotifier.Reply("Confirma mesmo a exclusão desse " +
            "produto?", "IMPORTANTE") == false) return validationStatus;

            return validationStatus = true;
        }

        public static bool ValidateToUpdate(ProductModel product)
        {
            var validationStatus = false;

            if (product.InternalCode == string.Empty)
            {
                MessageNotifier.Show("Informe o código interno " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.Description == string.Empty)
            {
                MessageNotifier.Show("Informe a descrição " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.Category.Id == 0)
            {
                MessageNotifier.Show("Selecione a categoria " +
                "do produto!", "Não Selecionada", "?");

                return validationStatus;
            }

            if (product.CostPrice == 0.00)
            {
                MessageNotifier.Show("Informe o preço de custo " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.SalePrice == 0.00)
            {
                MessageNotifier.Show("Informe o preço de venda " +
                "do produto!", "Campo Obrigatório", "?");

                return validationStatus;
            }

            if (product.CostPrice < 0.00)
            {
                MessageNotifier.Show("O preço de custo " +
                "do produto não pode ser negativo!",
                "Não Permitido", "?");

                return validationStatus;
            }

            if (product.SalePrice < 0.00)
            {
                MessageNotifier.Show("O preço de venda " +
                "do produto não pode ser negativo!",
                "Não Permitido", "?");

                return validationStatus;
            }

            if (product.CostPrice > product.SalePrice)
            {
                MessageNotifier.Show("O preço de custo " +
                "do produto não pode ser maior que o " +
                "preço de venda!", "Não Permitido", "?");

                return validationStatus;
            }

            if (product.CostPrice == product.SalePrice)
            {
                MessageNotifier.Show("O preço de custo " +
                "do produto não pode ser igual ao preço " +
                "de venda!", "Não Permitido", "?");

                return validationStatus;
            }

            if (product.CheckRegisterStatus() == false)
            {
                MessageNotifier.Show("Esse registro foi " +
                "excluido, atualize a lista de produtos!",
                "Atualize a Lista", "?");

                return validationStatus;
            }

            
            if (product.Stock.Min > product.Stock.Max)
            {
                MessageNotifier.Show("O estoque mínimo do produto " +
                "não pode ser maior que o estoque máximo!",
                "Não permitido", "?");

                return validationStatus;
            }

            if (product.Stock.Min > 0 && product.Stock.Min == product.Stock.Max)
            {
                MessageNotifier.Show("O estoque mínimo do produto " +
                "não pode ser igual ao estoque máximo!",
                "Não permitido", "?");

                return validationStatus;
            }

            if (ValidateChangerInCostPrice(product) == false)
                return validationStatus;

            return validationStatus = true;
        }

        public static bool ValidateChangerInCostPrice(ProductModel product)
        {
            var validationStatus = false;

            if (product.GetCostPrice() != product.CostPrice)
            {
                if (product.GetQuantityInStock() > 0)
                {
                    MessageNotifier.Show("Existem entradas no estoque " +
                    "relacionadas a esse produto e por isso, o preço " +
                    "de custo não poderá ser modificado até que todo " +
                    "estoque atual seje zerado!", "Não Permitido", "?");

                    return validationStatus;
                }
            }

            return validationStatus = true;
        }

        public static bool ValidateToGetDetails(ProductModel product)
        {
            var valitationStatus = false;

            if (product.Id == 0)
            {
                MessageNotifier.Show("Selecione o produto " +
                "para visualizar!", "Não Selecionado", "?");

                return valitationStatus;
            }

            if (product.CheckRegisterStatus() == false)
            {
                MessageNotifier.Show("Não é possivel visualizar " +
                "porque esse registro foi excluido!",
                "Atualize a Lista", "?");

                return valitationStatus;
            }

            return valitationStatus = true;
        }
    }
}
