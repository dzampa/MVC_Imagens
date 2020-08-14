using MVC_Imagens.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVC_Imagens.Controllers
{
    public class ProdutosController : Controller
    {
        ProdutoDbContext db;
        public ProdutosController()
        {
            db = new ProdutoDbContext();
        }
        // GET: Produtos
        public ActionResult Index()
        {
            ViewBag.Categorias = db.Categorias;
            var produtos = db.Produtos.ToList();
            return View(produtos);
        }

        public ActionResult Create()
        {
            ViewBag.Categorias = db.Categorias;
            var model = new ProdutoViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdutoViewModel model)
        {
            var imageTypes = new string[]
            {
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };
            if (model.ImageUpload == null || model.ImageUpload.ContentLength == 0)
                ModelState.AddModelError("ImageUpload", "Este campo é obrigatório");
            else if (!imageTypes.Contains(model.ImageUpload.ContentType))
                ModelState.AddModelError("ImageUpload", "Escolha uma imagem GIF, JPEG, PJPEG, PNG");
            if (ModelState.IsValid)
            {
                var produto = new Produto();
                produto.Nome = model.Nome;
                produto.Preco = model.Preco;
                produto.Descricao = model.Descricao;
                produto.CategoriaId = model.CategoriaId;

                //Salva a imagem para a pasta e pega o caminho
                var imagemNome = String.Format($"{DateTime.Now:yyyyMMdd-HHmmssfff}");
                var extensao = System.IO.Path.GetExtension(model.ImageUpload.FileName.ToLower());

                using (var img = System.Drawing.Image.FromStream(model.ImageUpload.InputStream))
                {
                    produto.Imagem = String.Format($"/ProdutoImagens/{imagemNome}{extensao}");
                    //Salva imagem
                    SalvarNaPasta(img, produto.Imagem);
                }

                db.Produtos.Add(produto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //Se ocorrer um erro retorna para pagina
            ViewBag.Categories = db.Categorias;
            return View(model);
        }

        private void SalvarNaPasta(Image img, string caminho)
        {
            using (System.Drawing.Image novaImagem = new Bitmap(img))
                novaImagem.Save(Server.MapPath(caminho), img.RawFormat);
        }

        // GET: Produtos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categorias = db.Categorias;
            return View(produto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome, Descricao, Preco, CategoriaId, Imagem, ProdutoId, Categoria")] Produto model)
        {
            if (ModelState.IsValid)
            {
                var produto = db.Produtos.Find(model.ProdutoId);
                if (produto == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produto.Nome = model.Nome;
                produto.Preco = model.Preco;
                produto.CategoriaId = model.CategoriaId;
                produto.Descricao = model.Descricao;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categorias = db.Categorias;
            return View(model);
        }

        // GET: Produtos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categorias = db.Categorias;
            return View(produto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "Nome, Descricao, Preco, CategoriaId, Imagem, ProdutoId, Categoria")] Produto model)
        {
            if (ModelState.IsValid)
            {
                var produto = db.Produtos.Find(model.ProdutoId);
                if (produto == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                produto.ProdutoId = model.ProdutoId;

                db.Produtos.Remove(produto);
                db.SaveChanges();

                ExcluirDaPasta(produto.Imagem);

                return RedirectToAction("Index");
            }
            ViewBag.Categorias = db.Categorias;
            return View(model);
        }
        private void ExcluirDaPasta(string caminho)
        {

            var filePath = Server.MapPath("~" + caminho);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // GET: Produtos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categorias = db.Categorias;
            return View(produto);
        }
    }
}