using Microsoft.AspNetCore.Mvc;
using TFTEC.Web.Ecommerce.Repositories;
using TFTEC.Web.Ecommerce.Repositories.Interfaces;

namespace TFTEC.Web.Ecommerce.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoController(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public IActionResult List()
        {
            var produtos = _produtoRepository.Produtos; 
            return View(produtos);
        }
    }
}
