using UnityEngine;


//Questa interfaccia è ereditata sia da CardUpgradeSO che da CardDowngradeSO
//il campo Name si trova qui perchè è la proprietà che viene usata nel gameloopmanager per identificare il singolo upgrade/downgrade (quando usa il metodo Find)
//le variabili booleane vengono usate nella funzione di check dell'upgrade/downgrade progressive, utili per indentificare se l'elemento analizzato è un upgrade o un downgrade usando un'unica funzione
public interface IEquipable
{
    string Name { get; }
    bool isUpgrade { get; }
    bool isDowngrade { get; }
}
