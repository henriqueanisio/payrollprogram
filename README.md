## Payroll Program
O Payroll Program é uma ferramenta para calcular o pagamento de funcionários. Ele considera dias trabalhados, valor da hora, dias e horas não trabalhadas, horas extras e dias extras.

## Funcionalidades

* Calcula o número de dias trabalhados seguindo a regra de segunda a sexta com 8 horas por dia mais 1 hora de almoço.
* Utiliza o valor da hora fornecido para calcular o pagamento.
* Desconta os dias e horas não trabalhados do pagamento.
* Adiciona o pagamento das horas extras e dias extras.
* Gera um arquivo com as seguintes informações:
* Valor pago por departamento.
* Dados da pessoa.
* Valor pago a cada pessoa.
* Valor descontado de cada pessoa.
* Quantidade de horas extras ou horas faltantes (com valor negativo quando aplicável).
* Quantidade de dias extras ou dias faltantes (com valor negativo quando aplicável).

## Tecnologias

Tecnologias utilizadas nesse projeto.

* .NET 6

## Como começar

* Para usar o Payroll Program, você precisará de documentos csv com formato de nome Departamento-Mes-Ano, o documento deverá conter as seguintes informações:

    - Codigo de cada funcionario
    - Nome de cada funcionario
    - Valor da hora de cada funcionário
    - Data de trabalho do funcionario do formato dd/MM/YYYY
    - Horario de entrada do funcionario no dia
    - Horario de saida do funcionario no dia
    - Periodo de almoco do funcionario - Ex: 12:00 - 13:00
  
* Para executar o projeto.
    - Informar a pasta de caminho dos arquivos csv
    - Informar a pasta de salvamento para o arquivo json

  ## Autor

  * **Henrique Anisio** 
