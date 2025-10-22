using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class PedidoRepository
{
    // EXERCÍCIO 7: Criar pedido com itens (transação)
    public void CriarPedido(Pedido pedido, List<PedidoItem> itens)
    {
        // TODO: Implemente criação de pedido com transação
        // 1. Inserir Pedido
        // 2. Inserir cada PedidoItem
        // 3. Atualizar estoque dos produtos
        // IMPORTANTE: Use SqlTransaction!

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            // TODO: Inicie a transação
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // TODO: 1. Inserir pedido e obter ID
                string sqlPedido = "INSERT INTO Pedidos (ClienteId, DataPedido, ValorTotal) " +
                                   "OUTPUT INSERTED.Id " +
                                   "VALUES (@ClienteId, @DataPedido, @ValorTotal)";

                int pedidoId = 0;
                using (SqlCommand cmd = new SqlCommand(sqlPedido, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", pedido.ClienteId);
                    cmd.Parameters.AddWithValue("@DataPedido", pedido.DataPedido);
                    cmd.Parameters.AddWithValue("@ValorTotal", pedido.ValorTotal);

                    // Executa e obtém o ID do pedido inserido
                    pedidoId = (int)cmd.ExecuteScalar();
                }

                // TODO: 2. Inserir itens do pedido
                string sqlItem = "INSERT INTO PedidoItens (PedidoId, ProdutoId, Quantidade, PrecoUnitario) " +
                                 "VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)";

                foreach (var item in itens)
                {
                    using (SqlCommand cmd = new SqlCommand(sqlItem, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@PrecoUnitario", item.PrecoUnitario);
                        cmd.ExecuteNonQuery();
                    }

                    // TODO: 3. Atualizar estoque
                    string sqlEstoque = "UPDATE Produtos SET Estoque = Estoque - @Quantidade WHERE Id = @ProdutoId";
                    using (SqlCommand cmd = new SqlCommand(sqlEstoque, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.ProdutoId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // TODO: Commit da transação
                transaction.Commit();
                Console.WriteLine("Pedido criado com sucesso!");
            }
            catch (Exception ex)
            {
                // TODO: Rollback em caso de erro
                transaction.Rollback();
                Console.WriteLine($"Erro ao criar pedido: {ex.Message}");
                throw;
            }
        }
    }

    // EXERCÍCIO 8: Listar pedidos de um cliente
    public void ListarPedidosCliente(int clienteId)
    {
        // TODO: Liste todos os pedidos de um cliente
        // Mostre: Id, Data, ValorTotal

        string sql = "SELECT * FROM Pedidos WHERE ClienteId = @ClienteId ORDER BY DataPedido DESC";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ClienteId", clienteId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("ID | DataPedido | ValorTotal");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} | " +
                                          $"{(reader["DataPedido"] != DBNull.Value ? Convert.ToDateTime(reader["DataPedido"]).ToString("dd/MM/yyyy") : string.Empty)} | " +
                                          $"{(reader["ValorTotal"] != DBNull.Value ? Convert.ToDecimal(reader["ValorTotal"]) : 0m)}");
                    }
                }
            }
        }
    }

    // EXERCÍCIO 9: Obter detalhes completos de um pedido
    public void ObterDetalhesPedido(int pedidoId)
    {
        // TODO: Mostre o pedido com todos os itens
        // Faça JOIN com Produtos para mostrar nomes

        string sql = @"SELECT 
                            pi.*, 
                            p.Nome as NomeProduto,
                            (pi.Quantidade * pi.PrecoUnitario) as Subtotal
                        FROM PedidoItens pi
                        INNER JOIN Produtos p ON pi.ProdutoId = p.Id
                        WHERE pi.PedidoId = @PedidoId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Produto | Quantidade | PrecoUnitario | Subtotal");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["NomeProduto"] as string ?? string.Empty} | " +
                                          $"{(reader["Quantidade"] != DBNull.Value ? Convert.ToInt32(reader["Quantidade"]) : 0)} | " +
                                          $"{(reader["PrecoUnitario"] != DBNull.Value ? Convert.ToDecimal(reader["PrecoUnitario"]) : 0m)} | " +
                                          $"{(reader["Subtotal"] != DBNull.Value ? Convert.ToDecimal(reader["Subtotal"]) : 0m)}");
                    }
                }
            }
        }
    }

    // DESAFIO 3: Calcular total de vendas por período
    public void TotalVendasPorPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        // TODO: Calcule o total de vendas em um período
        // Use ExecuteScalar para obter a soma
        string sql = "SELECT SUM(ValorTotal) FROM Pedidos WHERE DataPedido BETWEEN @DataInicio AND @DataFim";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                var resultado = cmd.ExecuteScalar();
                decimal total = (resultado != DBNull.Value ? Convert.ToDecimal(resultado) : 0m);
                Console.WriteLine($"Total de vendas de {dataInicio:dd/MM/yyyy} até {dataFim:dd/MM/yyyy}: {total:C}");
            }
        }
    }
}
