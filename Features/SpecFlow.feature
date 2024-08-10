Feature: Pesquisa de CEP e Rastreio de Código

  Scenario: Validar CEP e Código de Rastreamento
    Given que eu estou no site dos Correios
    When eu procuro pelo CEP "80700000"
    Then o CEP deve ser inexistente
    When eu procuro pelo CEP "01013-001"
    Then o resultado deve ser "Rua Quinze de Novembro, São Paulo/SP"
    When eu procuro no rastreamento pelo código "SS987654321BR"
    Then o código deve estar incorreto
