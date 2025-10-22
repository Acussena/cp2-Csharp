

using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using SistemaLoja.Lab12_ConexaoSQLServer;

namespace SistemaLoja
{
    // ===============================================
    // MODELOS DE DADOS
    // ===============================================

    // ===============================================
    // CLASSE DE CONEXÃO
    // ===============================================

    // ===============================================
    // REPOSITÓRIO DE PRODUTOS
    // ===============================================

    // ===============================================
    // REPOSITÓRIO DE PEDIDOS
    // ===============================================

    // ===============================================
    // CLASSE PRINCIPAL
    // ===============================================

    class Program
    {
        static void Main(string[] args)
        {
            // IMPORTANTE: Antes de executar, crie o banco de dados!
            // Execute o script SQL fornecido no arquivo setup.sql

            Console.WriteLine("=== LAB 12 - CONEXÃO SQL SERVER ===\n");

            var produtoRepo = new ProdutoRepository();
            var pedidoRepo = new PedidoRepository();

            bool continuar = true;

            while (continuar)
            {
                MostrarMenu();
                string opcao = Console.ReadLine() ?? "";

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            produtoRepo.ListarTodosProdutos();
                            break;
                        case "2":
                            InserirNovoProduto(produtoRepo);
                            break;
                        case "3":
                            AtualizarProdutoExistente(produtoRepo);
                            break;
                        case "4":
                            DeletarProdutoExistente(produtoRepo);
                            break;
                        case "5":
                            ListarPorCategoria(produtoRepo);
                            break;
                        case "6":
                            CriarNovoPedido(pedidoRepo);
                            break;
                        case "7":
                            ListarPedidosDeCliente(pedidoRepo);
                            break;
                        case "8":
                            DetalhesDoPedido(pedidoRepo);
                            break;
                        case "0":
                            continuar = false;
                            break;
                        default:
                            Console.WriteLine("Opção inválida!");
                            break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"\n❌ Erro SQL: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Erro: {ex.Message}");
                }

                if (continuar)
                {
                    Console.WriteLine("\nPressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            Console.WriteLine("\nPrograma finalizado!");
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║       MENU PRINCIPAL               ║");
            Console.WriteLine("╠════════════════════════════════════╣");
            Console.WriteLine("║  PRODUTOS                          ║");
            Console.WriteLine("║  1 - Listar todos os produtos      ║");
            Console.WriteLine("║  2 - Inserir novo produto          ║");
            Console.WriteLine("║  3 - Atualizar produto             ║");
            Console.WriteLine("║  4 - Deletar produto               ║");
            Console.WriteLine("║  5 - Listar por categoria          ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  PEDIDOS                           ║");
            Console.WriteLine("║  6 - Criar novo pedido             ║");
            Console.WriteLine("║  7 - Listar pedidos de cliente     ║");
            Console.WriteLine("║  8 - Detalhes de um pedido         ║");
            Console.WriteLine("║                                    ║");
            Console.WriteLine("║  0 - Sair                          ║");
            Console.WriteLine("╚════════════════════════════════════╝");
            Console.Write("\nEscolha uma opção: ");
        }

        // ===============================================
        // MÉTODOS AUXILIARES
        // ===============================================

        static void InserirNovoProduto(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== INSERIR NOVO PRODUTO ===");

            Console.Write("Nome: ");
            string nome = Console.ReadLine() ?? "";

            Console.Write("Preço: ");
            decimal preco = decimal.TryParse(Console.ReadLine(), out var p) ? p : 0m;

            Console.Write("Estoque: ");
            int estoque = int.TryParse(Console.ReadLine(), out var e) ? e : 0;

            Console.Write("Categoria ID: ");
            int categoriaId = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

            var produto = new Produto
            {
                Nome = nome,
                Preco = preco,
                Estoque = estoque,
                CategoriaId = categoriaId
            };

            repo.InserirProduto(produto);
        }

        static void AtualizarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== ATUALIZAR PRODUTO ===");

            Console.Write("ID do produto: ");
            int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

            Produto? produto = repo.BuscarPorId(id);
            if (produto == null)
            {
                Console.WriteLine("Produto não encontrado!");
                return;
            }

            Console.Write($"Nome ({produto.Nome}): ");
            string nome = Console.ReadLine() ?? produto.Nome;

            Console.Write($"Preço ({produto.Preco}): ");
            decimal preco = decimal.TryParse(Console.ReadLine(), out var p) ? p : produto.Preco;

            Console.Write($"Estoque ({produto.Estoque}): ");
            int estoque = int.TryParse(Console.ReadLine(), out var e) ? e : produto.Estoque;

            produto.Nome = nome;
            produto.Preco = preco;
            produto.Estoque = estoque;

            repo.AtualizarProduto(produto);
        }

        static void DeletarProdutoExistente(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== DELETAR PRODUTO ===");

            Console.Write("ID do produto: ");
            int id = int.TryParse(Console.ReadLine(), out var i) ? i : 0;

            Console.Write("Tem certeza que deseja deletar? (s/n): ");
            string confirm = Console.ReadLine() ?? "n";

            if (confirm.ToLower() == "s")
            {
                repo.DeletarProduto(id);
            }
        }

        static void ListarPorCategoria(ProdutoRepository repo)
        {
            Console.WriteLine("\n=== PRODUTOS POR CATEGORIA ===");

            Console.Write("ID da categoria: ");
            int categoriaId = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

            repo.ListarProdutosPorCategoria(categoriaId);
        }

        static void CriarNovoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== CRIAR NOVO PEDIDO ===");

            Console.Write("ID do cliente: ");
            int clienteId = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

            Console.Write("Quantidade de itens: ");
            int qtdItens = int.TryParse(Console.ReadLine(), out var q) ? q : 0;

            List<PedidoItem> itens = new List<PedidoItem>();
            decimal valorTotal = 0m;

            for (int i = 0; i < qtdItens; i++)
            {
                Console.WriteLine($"\nItem {i + 1}:");
                Console.Write("ID do produto: ");
                int produtoId = int.TryParse(Console.ReadLine(), out var pid) ? pid : 0;

                Console.Write("Quantidade: ");
                int quantidade = int.TryParse(Console.ReadLine(), out var qt) ? qt : 0;

                Console.Write("Preço unitário: ");
                decimal precoUnitario = decimal.TryParse(Console.ReadLine(), out var pu) ? pu : 0m;

                itens.Add(new PedidoItem
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    PrecoUnitario = precoUnitario
                });

                valorTotal += quantidade * precoUnitario;
            }

            var pedido = new Pedido
            {
                ClienteId = clienteId,
                DataPedido = DateTime.Now,
                ValorTotal = valorTotal
            };

            repo.CriarPedido(pedido, itens);
        }

        static void ListarPedidosDeCliente(PedidoRepository repo)
        {
            Console.WriteLine("\n=== PEDIDOS DO CLIENTE ===");

            Console.Write("ID do cliente: ");
            int clienteId = int.TryParse(Console.ReadLine(), out var c) ? c : 0;

            repo.ListarPedidosCliente(clienteId);
        }

        static void DetalhesDoPedido(PedidoRepository repo)
        {
            Console.WriteLine("\n=== DETALHES DO PEDIDO ===");

            Console.Write("ID do pedido: ");
            int pedidoId = int.TryParse(Console.ReadLine(), out var p) ? p : 0;

            repo.ObterDetalhesPedido(pedidoId);
        }
    }
}