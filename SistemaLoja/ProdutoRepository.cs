using Microsoft.Data.SqlClient;

namespace SistemaLoja.Lab12_ConexaoSQLServer;

public class ProdutoRepository
{
    // EXERCÍCIO 1: Listar todos os produtos
    public void ListarTodosProdutos()
    {
        // TODO: Implemente a listagem de produtos
        // Dica: Use ExecuteReader e while(reader.Read())

        string sql = "SELECT * FROM Produtos";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                // TODO: Execute o comando e leia os resultados
                // Mostre: Id, Nome, Preco, Estoque
                Console.WriteLine("ID | Nome | Preço | Estoque");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]} | {reader["Nome"] as string ?? string.Empty} | " +
                                      $"{(reader["Preco"] != DBNull.Value ? Convert.ToDecimal(reader["Preco"]) : 0m)} | " +
                                      $"{(reader["Estoque"] != DBNull.Value ? Convert.ToInt32(reader["Estoque"]) : 0)}");
                }
            }
        }
    }

    // EXERCÍCIO 2: Inserir novo produto
    public void InserirProduto(Produto produto)
    {
        // TODO: Implemente a inserção de produto
        // IMPORTANTE: Use parâmetros para evitar SQL Injection!

        string sql = "INSERT INTO Produtos (Nome, Preco, Estoque, CategoriaId) " +
                     "VALUES (@Nome, @Preco, @Estoque, @CategoriaId)";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // TODO: Adicione os parâmetros
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);

                // TODO: Execute o comando
                int linhasAfetadas = cmd.ExecuteNonQuery();

                Console.WriteLine(linhasAfetadas > 0 ? "Produto inserido com sucesso!" : "Falha ao inserir produto.");
            }
        }
    }

    // EXERCÍCIO 3: Atualizar produto
    public void AtualizarProduto(Produto produto)
    {
        // TODO: Implemente a atualização de produto
        // Dica: UPDATE Produtos SET ... WHERE Id = @Id

        string sql = "UPDATE Produtos SET " +
                     "Nome = @Nome, " +
                     "Preco = @Preco, " +
                     "Estoque = @Estoque, " +
                     "CategoriaId = @CategoriaId " +
                     "WHERE Id = @Id";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            // TODO: Complete a implementação
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Estoque", produto.Estoque);
                cmd.Parameters.AddWithValue("@CategoriaId", produto.CategoriaId);
                cmd.Parameters.AddWithValue("@Id", produto.Id);

                int linhasAfetadas = cmd.ExecuteNonQuery();
                Console.WriteLine(linhasAfetadas > 0 ? "Produto atualizado!" : "Produto não encontrado.");
            }
        }
    }

    // EXERCÍCIO 4: Deletar produto
    public void DeletarProduto(int id)
    {
        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            // 1. Verificar se o produto está vinculado a algum pedido
            string checkSql = "SELECT COUNT(*) FROM PedidoItens WHERE ProdutoId = @Id";
            using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
            {
                checkCmd.Parameters.AddWithValue("@Id", id);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    Console.WriteLine("Produto não pode ser deletado: está vinculado a pedidos.");
                    return;
                }
            }

            // 2. Deletar produto
            string deleteSql = "DELETE FROM Produtos WHERE Id = @Id";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn))
            {
                deleteCmd.Parameters.AddWithValue("@Id", id);
                int linhasAfetadas = deleteCmd.ExecuteNonQuery();
                Console.WriteLine(linhasAfetadas > 0 ? "✅ Produto deletado!" : "Produto não encontrado.");
            }
        }
    }


    // EXERCÍCIO 5: Buscar produto por ID
    public Produto? BuscarPorId(int id)
    {
        // TODO: Implemente a busca por ID
        // Retorne um objeto Produto ou null se não encontrar

        string sql = "SELECT * FROM Produtos WHERE Id = @Id";
        Produto? produto = null;

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // TODO: Preencha o objeto produto com os dados
                        produto = new Produto
                        {
                            Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                            Nome = reader["Nome"] as string ?? string.Empty,
                            Preco = reader["Preco"] != DBNull.Value ? Convert.ToDecimal(reader["Preco"]) : 0m,
                            Estoque = reader["Estoque"] != DBNull.Value ? Convert.ToInt32(reader["Estoque"]) : 0,
                            CategoriaId = reader["CategoriaId"] != DBNull.Value ? Convert.ToInt32(reader["CategoriaId"]) : 0
                        };
                    }
                }
            }
        }

        return produto;
    }

    // EXERCÍCIO 6: Listar produtos por categoria
    public void ListarProdutosPorCategoria(int categoriaId)
    {
        // TODO: Implemente a listagem filtrada por categoria
        // Dica: Faça um JOIN com a tabela Categorias para mostrar o nome

        string sql = @"SELECT p.Id, p.Nome, p.Preco, p.Estoque, c.Nome as NomeCategoria 
                          FROM Produtos p
                          INNER JOIN Categorias c ON p.CategoriaId = c.Id
                          WHERE p.CategoriaId = @CategoriaId";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoriaId", categoriaId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("ID | Nome | Preço | Estoque | Categoria");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} | {reader["Nome"] as string ?? string.Empty} | " +
                                          $"{(reader["Preco"] != DBNull.Value ? Convert.ToDecimal(reader["Preco"]) : 0m)} | " +
                                          $"{(reader["Estoque"] != DBNull.Value ? Convert.ToInt32(reader["Estoque"]) : 0)} | " +
                                          $"{reader["NomeCategoria"] as string ?? string.Empty}");
                    }
                }
            }
        }
    }

    // DESAFIO 1: Buscar produtos com estoque baixo
    public void ListarProdutosEstoqueBaixo(int quantidadeMinima)
    {
        // TODO: Liste produtos com estoque menor que quantidadeMinima
        // Mostre um alerta visual para chamar atenção
        string sql = "SELECT * FROM Produtos WHERE Estoque < @QuantidadeMinima";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@QuantidadeMinima", quantidadeMinima);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Produtos com estoque baixo:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ALERTA! {reader["Nome"] as string ?? string.Empty} - Estoque: " +
                                          $"{(reader["Estoque"] != DBNull.Value ? Convert.ToInt32(reader["Estoque"]) : 0)}");
                    }
                }
            }
        }
    }

    // DESAFIO 2: Buscar produtos por nome (LIKE)
    public void BuscarProdutosPorNome(string termoBusca)
    {
        // TODO: Implemente busca com LIKE
        // Dica: Use '%' + termoBusca + '%'
        string sql = "SELECT * FROM Produtos WHERE Nome LIKE @Termo";

        using (SqlConnection conn = DatabaseConnection.GetConnection())
        {
            conn.Open();
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Termo", "%" + termoBusca + "%");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine($"Produtos que contêm '{termoBusca}':");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]} | {reader["Nome"] as string ?? string.Empty} | " +
                                          $"{(reader["Preco"] != DBNull.Value ? Convert.ToDecimal(reader["Preco"]) : 0m)} | " +
                                          $"{(reader["Estoque"] != DBNull.Value ? Convert.ToInt32(reader["Estoque"]) : 0)}");
                    }
                }
            }
        }
    }
}
