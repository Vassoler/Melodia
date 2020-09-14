﻿public class Nivel
{
    private int id;
    private string nome;
    private string descricao;
    private Dificuldade dificuldade;

    public int Id
    {
        get
        {
            return id;
        }
        set
        {
            id = value;
        }
    }

    public string Nome
    {
        get
        {
            return nome;
        }
        set
        {
            nome = value;
        }
    }

    public string Descricao
    {
        get
        {
            return descricao;
        }
        set
        {
            descricao = value;
        }
    }

    public Dificuldade Dificuldade
    {
        get
        {
            return dificuldade;
        }
        set
        {
            dificuldade = value;
        }
    }
}