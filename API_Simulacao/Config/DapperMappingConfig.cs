using API_Simulacao.Models;
using Dapper;

namespace API_Simulacao.Config
{
    public static class DapperMappingConfig
    {
        public static void Configure()
        {
            ProdutoDapperMapping.Map();
            SimulacaoDapperMapping.Map();
            ParcelasDapperMapping.Map();
        }
    }

    public static class ProdutoDapperMapping
    {
        public static void Map()
        {
            SqlMapper.SetTypeMap(typeof(Produto), new CustomPropertyTypeMap(
                typeof(Produto),
                (type, columnName) => type.GetProperties().FirstOrDefault(
                    prop => prop.Name.Equals(MapColumnToProperty(columnName), StringComparison.OrdinalIgnoreCase))
            ));
        }

        private static string MapColumnToProperty(string columnName) => columnName switch
        {
            "CO_PRODUTO" => "CoProduto",
            "NO_PRODUTO" => "NomeProduto",
            "PC_TAXA_JUROS" => "PcTaxaJuros",
            "NU_MINIMO_MESES" => "NuMinimoMeses",
            "NU_MAXIMO_MESES" => "NuMaximoMeses",
            "VR_MINIMO" => "VrMinimo",
            "VR_MAXIMO" => "VrMaximo",
            _ => columnName
        };
    }

    public static class SimulacaoDapperMapping
    {
        public static void Map()
        {
            SqlMapper.SetTypeMap(typeof(Produto), new CustomPropertyTypeMap(
                typeof(Produto),
                (type, columnName) => type.GetProperties().FirstOrDefault(
                    prop => prop.Name.Equals(MapColumnToProperty(columnName), StringComparison.OrdinalIgnoreCase))
            ));
        }

        private static string MapColumnToProperty(string columnName) => columnName switch
        {
            "ID" => "Id",
            "TIPO" => "Tipo",
            "DATA_CRIACAO" => "DataCriacao",
            _ => columnName
        };
    }
    
    public static class ParcelasDapperMapping
    {
        public static void Map()
        {
            SqlMapper.SetTypeMap(typeof(Produto), new CustomPropertyTypeMap(
                typeof(Produto),
                (type, columnName) => type.GetProperties().FirstOrDefault(
                    prop => prop.Name.Equals(MapColumnToProperty(columnName), StringComparison.OrdinalIgnoreCase))
            ));
        }

        private static string MapColumnToProperty(string columnName) => columnName switch
        {
            "ID" => "Id",
            "NUMERO" => "Numero",
            "VALOR_AMORTIZACAO" => "ValorAmortizacao",
            "VALOR_JUROS" => "ValorJuros",
            "VALOR_PRESTACAO" => "ValorPrestacao",
            _ => columnName
        };
    }

}
