import { Component } from '@angular/core';
import { CalculoCdbService } from './_services/calculo-cdb.service';
import { CalculoCdbDto } from './_models/calculo-cdb-dto.model';
import { CalculoCdb } from './_models/calculo-cdb.model'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {

  valorInicial: number = 0.0;
  quantidadeDeMeses: number = 0;

  calculoResposta!: CalculoCdbDto;

  constructor(private servico: CalculoCdbService) { }


  registraValorInicial(valor: string) {
    this.valorInicial = parseFloat(valor);
  }

  registraQuantidadeDeMeses(valor: string) {
    this.quantidadeDeMeses = parseInt(valor);
  }

  calcular() {
    const calculoCdb: CalculoCdb = {
      valorInicial: this.valorInicial,
      quantidadeDeMeses: this.quantidadeDeMeses
    };

    this.servico.calculaCdb(calculoCdb)
      .subscribe(resposta => this.calculoResposta = resposta);

    console.log('Valor Inicial: ', this.valorInicial);
    console.log('Quantidade de Meses: ', this.quantidadeDeMeses);
  }
}
