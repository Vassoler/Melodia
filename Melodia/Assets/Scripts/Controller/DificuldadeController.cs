﻿public class DificuldadeController
{
    DificuldadeModel model;

    public DificuldadeController()
    {
        model = new DificuldadeModel();
    }

    public Dificuldade get(int id)
    {
        return model.get(id);
    }
}