# 🚀 Demo NFe com Zeus Fiscal

## 📋 Sobre o Projeto

Este é um projeto de demonstração que implementa a emissão de **Nota Fiscal Eletrônica (NFe)** utilizando a biblioteca [Zeus Fiscal](https://github.com/Hercules-NET/ZeusFiscal). O projeto demonstra o processo completo de:

- ✅ Configuração do ambiente NFe
- ✅ Geração de NFe
- ✅ Assinatura digital
- ✅ Validação XML
- ✅ Envio para SEFAZ
- ✅ Geração de DANFE (PDF)

## 🎯 Funcionalidades

- **Emissão de NFe 4.0**: Compatível com o layout mais atual
- **Assinatura Digital**: Suporte a certificados A1 (PFX)
- **Validação Automática**: Validação de schemas XML
- **Impressão DANFE**: Geração de PDF com FastReport
- **Ambiente de Homologação**: Configurado para testes seguros
- **Visualização Automática**: Abertura automática de XMLs e PDFs

## 🛠️ Tecnologias Utilizadas

- **.NET 9**: Framework mais recente
- **C# 13.0**: Linguagem de programação
- **Zeus Fiscal (Hercules.NET.NFe.NFCe)**: Biblioteca principal para NFe
- **FastReport OpenSource**: Para geração de relatórios DANFE
- **System.Security.Cryptography.Xml**: Para assinatura digital

## 📦 Dependências

```xml
<PackageReference Include="Hercules.NET.NFe.NFCe" Version="2025.7.15.1635" />
<PackageReference Include="System.Security.Cryptography.Xml" Version="9.0.7" />
<PackageReference Include="FastReport.OpenSource" Version="2022.2.2" />
<PackageReference Include="FastReport.OpenSource.Export.PdfSimple" Version="2022.2.2" />
```

## ⚡ Quick Start

### 1. Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio 2022 ou VS Code
- Certificado digital A1 (PFX) válido
- Acesso à internet para comunicação com SEFAZ

### 2. Configuração do Certificado

1. Coloque seu certificado `.pfx` no diretório do projeto
2. Edite o arquivo `Program.cs` na seção de configuração:

    ```csharp
    Certificado = new DFe.Utils.ConfiguracaoCertificado()
    {
        TipoCertificado = DFe.Utils.TipoCertificado.A1ByteArray,
        ArrayBytesArquivo = File.ReadAllBytes("CAMINHO/PARA/SEU/CERTIFICADO.pfx"),
        Senha = "SUA_SENHA_AQUI",
        // ... outras configurações
    }
    ```

### 3. Configuração do Estado

Ajuste as configurações para seu estado:

```csharp
var configuracao = new ConfiguracaoServico()
{
    cUF = DFe.Classes.Entidades.Estado.SP, // Altere para seu estado
    tpAmb = DFe.Classes.Flags.TipoAmbiente.Homologacao, // Ou Producao
    // ... outras configurações
}
```

### 4. Executar o Projeto

```bash
dotnet run
```

## 📁 Estrutura do Projeto

```
HerculesZeusDfeDemo/
├── Program.cs              # Arquivo principal com fluxo de emissão
├── FactoryNfe.cs          # Factory para criação de NFe
├── Helpers.cs             # Utilitários para visualização
├── Schemas/               # Schemas XSD para validação
├── NFe/                   # Templates de relatórios
│   └── NFeRetrato.frx     # Template DANFE
├── *.dll                  # Bibliotecas do DANFE
└── HerculesZeusDfeDemo.csproj
```

## 🔧 Configurações Principais

### ConfiguracaoServico

| Propriedade | Descrição | Valor Padrão |
|-------------|-----------|--------------|
| `tpAmb` | Ambiente (Homologação/Produção) | `Homologacao` |
| `cUF` | Estado emissor | `SP` |
| `VersaoLayout` | Versão do layout NFe | `Versao400` |
| `TimeOut` | Timeout para requisições | `20000ms` |
| `ValidarSchemas` | Validar XML contra schemas | `false` |

### Emitente (FactoryNfe.cs)

Configurações do emitente da NFe:
- **CNPJ**: 62559695000101
- **Razão Social**: BREDA'S INFORMATICA LTDA
- **Nome Fantasia**: BREDAS SISTEMAS
- **Regime Tributário**: Simples Nacional

## 🔄 Fluxo de Emissão

1. **Configuração**: Carregamento de certificado e parâmetros
2. **Geração**: Criação da estrutura XML da NFe
3. **Visualização**: Exibição do XML não assinado
4. **Assinatura**: Aplicação da assinatura digital
5. **Validação**: Verificação contra schemas XSD
6. **Envio**: Transmissão para SEFAZ
7. **Protocolo**: Recebimento do protocolo de autorização
8. **DANFE**: Geração e exibição do PDF

## 📊 Dados da NFe de Exemplo

- **Série**: 32
- **Número**: 117
- **Produto**: Água Mineral (R$ 10,00)
- **NCM**: 22021000
- **CFOP**: 5102
- **Regime**: Simples Nacional (CSOSN 102)

## 🚨 Importante

⚠️ **ATENÇÃO**: Este projeto está configurado para o **ambiente de homologação**. Para usar em produção:

1. Altere `tpAmb` para `Producao`
2. Configure um certificado válido para produção
3. Ajuste os dados do emitente para dados reais
4. Teste exaustivamente antes de usar

## 🐛 Solução de Problemas

### Certificado não encontrado
```
Erro: Arquivo de certificado não encontrado
Solução: Verifique o caminho do arquivo .pfx
```

### Erro de validação
```
Erro: XML inválido
Solução: Verifique se os schemas estão na pasta Schemas/
```

### Timeout na SEFAZ
```
Erro: Timeout ao comunicar com SEFAZ
Solução: Verifique conexão com internet e firewall
```

## 📚 Recursos Adicionais

- 🎥 **Vídeo Tutorial**: [YouTube - Demo Zeus Fiscal](https://www.youtube.com/watch?v=3i06uBOfgSE)
- 💬 **Discord**: [Comunidade Hercules](https://discord.gg/EE4TGKAkkG)
- 📖 **Documentação**: [Zeus Fiscal GitHub](https://github.com/Hercules-NET/ZeusFiscal)
- 📋 **Manual SEFAZ**: [Portal NFe](http://www.nfe.fazenda.gov.br/)

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto é distribuído sob a licença MIT. Veja `LICENSE` para mais informações.

## 👨‍💻 Autor

**Breda's Informática**
- 🌐 Website: [bredas.com.br](https://bredas.com.br)
- 📧 Email: bredas@bredas.com.br
- 📞 Telefone: (14) 3413-3244

---

⭐ **Gostou do projeto?** Dê uma estrela no repositório!

🔔 **Mantenha-se atualizado** sobre as novidades do Zeus Fiscal seguindo o projeto no GitHub.
