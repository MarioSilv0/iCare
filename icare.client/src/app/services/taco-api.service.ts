import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { env } from '../../environments/env';

@Injectable({
  providedIn: 'root'
})
export class TacoApiService {
  tacoApi = env.tacoApiUrl;

  constructor(private http: HttpClient) { }

  //funciona com o filtro da API mas é muito strict recomendo fazer filtro do zero sobre o getAllFood
  searchFood(query: string): Observable<any> {
    const graphqlQuery = {
      query: `
        query {
          getFoodByName(name: "${query}") {
              name
              category {
                name
              }
              nutrients {
                kJ
                protein
                lipids
                kcal
                dietaryFiber
                carbohydrates
              }
          }
        }
      `
    };
    return this.http.post(this.tacoApi, graphqlQuery);
  }

  getAllFood(): Observable < any > {
    const graphqlQuery = {
      query: `
        query {
          getAllFood {
              name
              category {
                name
              }
              nutrients {
                kJ
                protein
                lipids
                kcal
                dietaryFiber
                carbohydrates
              }
          }
        }
      `
    };
    console.log(this.tacoApi);
    return this.http.post(this.tacoApi, graphqlQuery);
   }


}
